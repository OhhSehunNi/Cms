using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cms.Infrastructure.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly List<string> _cacheKeys = new List<string>();

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out T value))
            {
                return value;
            }
            return default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
            {
                cacheEntryOptions.SetSlidingExpiration(expiration.Value);
            }
            _cache.Set(key, value, cacheEntryOptions);
            
            // 记录缓存键，用于后续的模式匹配删除
            if (!_cacheKeys.Contains(key))
            {
                _cacheKeys.Add(key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            _cacheKeys.Remove(key);
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            var keysToRemove = _cacheKeys.Where(key => System.Text.RegularExpressions.Regex.IsMatch(key, pattern)).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                _cacheKeys.Remove(key);
            }
        }
    }
}