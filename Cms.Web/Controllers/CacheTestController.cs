using Microsoft.AspNetCore.Mvc;
using Cms.Infrastructure.Services;

namespace Cms.Web.Controllers
{
    /// <summary>
    /// 缓存测试控制器
    /// 用于测试缓存服务的功能
    /// </summary>
    public class CacheTestController : Controller
    {
        /// <summary>
        /// 缓存服务接口
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cacheService">缓存服务实例</param>
        public CacheTestController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        /// <summary>
        /// 缓存测试首页
        /// 测试设置和获取缓存
        /// </summary>
        /// <returns>缓存测试视图</returns>
        public async Task<IActionResult> Index()
        {
            // 测试设置缓存
            await _cacheService.SetAsync("test:key", "Hello Redis!", TimeSpan.FromMinutes(5));
            
            // 测试获取缓存
            var value = await _cacheService.GetAsync<string>("test:key");
            
            ViewBag.CacheValue = value;
            return View();
        }

        /// <summary>
        /// 移除缓存
        /// 测试删除缓存功能
        /// </summary>
        /// <returns>缓存测试视图</returns>
        public async Task<IActionResult> Remove()
        {
            // 测试删除缓存
            await _cacheService.RemoveAsync("test:key");
            
            // 验证缓存是否已删除
            var value = await _cacheService.GetAsync<string>("test:key");
            
            ViewBag.CacheValue = value;
            return View("Index");
        }
    }
}
