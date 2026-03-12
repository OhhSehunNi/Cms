using Newtonsoft.Json;
using StackExchange.Redis;

namespace Cms.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _database;

        public RedisCacheService(string connectionString)
        {
            var connection = ConnectionMultiplexer.Connect(connectionString);
            _database = connection.GetDatabase();
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            if (value.IsNull)
                return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var jsonValue = JsonConvert.SerializeObject(value);
            await _database.StringSetAsync(key, jsonValue, expiration ?? TimeSpan.FromHours(1));
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            var connection = ConnectionMultiplexer.Connect("localhost:6379");
            var server = connection.GetServer("localhost", 6379);
            var keys = server.Keys(pattern: pattern);
            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }
        }
    }
}