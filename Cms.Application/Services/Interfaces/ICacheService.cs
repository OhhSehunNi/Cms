using System;

namespace Cms.Application.Services
{
    /// <summary>
    /// 缓存服务接口
    /// </summary>
    public interface ICacheService
    {
        void Set<T>(string key, T value, TimeSpan expiration);
        T Get<T>(string key);
        void Remove(string key);
        bool Exists(string key);
        int Increment(string key, TimeSpan expiration);
    }
}