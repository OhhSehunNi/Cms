using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Cms.Application.Services
{
    /// <summary>
    /// 缓存服务，用于封装Memory Cache操作
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="memoryCache">内存缓存实例</param>
        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiration">过期时间</param>
        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            _memoryCache.Set(key, value, expiration);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值，不存在则返回默认值</returns>
        public T Get<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T value);
            return value;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        /// <summary>
        /// 检查缓存是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否存在</returns>
        public bool Exists(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }

        /// <summary>
        /// 递增计数器
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="expiration">过期时间</param>
        /// <returns>递增后的值</returns>
        public int Increment(string key, TimeSpan expiration)
        {
            var current = Get<int>(key);
            var newValue = current + 1;
            Set(key, newValue, expiration);
            return newValue;
        }
    }
}