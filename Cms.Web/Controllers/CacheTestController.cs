using Microsoft.AspNetCore.Mvc;
using Cms.Infrastructure.Services;

namespace Cms.Web.Controllers
{
    public class CacheTestController : Controller
    {
        private readonly ICacheService _cacheService;

        public CacheTestController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<IActionResult> Index()
        {
            // 测试设置缓存
            await _cacheService.SetAsync("test:key", "Hello Redis!", TimeSpan.FromMinutes(5));
            
            // 测试获取缓存
            var value = await _cacheService.GetAsync<string>("test:key");
            
            ViewBag.CacheValue = value;
            return View();
        }

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
