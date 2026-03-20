using Microsoft.Extensions.Caching.Memory;
using Cms.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cms.Infrastructure.Services
{
    public class MemoryCacheService : Cms.Domain.Services.ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly List<string> _cacheKeys = new List<string>();

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out T value))
            {
                return value;
            }
            return default;
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(expiration);
            _cache.Set(key, value, cacheEntryOptions);
            
            // 记录缓存键，用于后续的模式匹配删除
            if (!_cacheKeys.Contains(key))
            {
                _cacheKeys.Add(key);
            }
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _cacheKeys.Remove(key);
        }

        public bool Exists(string key)
        {
            return _cache.TryGetValue(key, out _);
        }

        public int Increment(string key, TimeSpan expiration)
        {
            int value = 0;
            if (_cache.TryGetValue(key, out int currentValue))
            {
                value = currentValue + 1;
            }
            else
            {
                value = 1;
            }
            Set(key, value, expiration);
            return value;
        }
    }
}