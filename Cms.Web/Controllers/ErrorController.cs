using Microsoft.AspNetCore.Mvc;
using Cms.Application.Services;

namespace Cms.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IWebsiteService _websiteService;

        public ErrorController(IWebsiteService websiteService)
        {
            _websiteService = websiteService;
        }

        [Route("error/404")]
        public async Task<IActionResult> NotFoundPage()
        {
            // 从当前请求上下文中获取WebsiteId
            var websiteId = HttpContext.Items["WebsiteId"] as int? ?? 1;
            
            // 获取网站信息
            var website = await _websiteService.GetByIdAsync(websiteId);
            ViewBag.WebsiteName = website?.Name ?? "新闻网站";
            ViewBag.FooterInfo = website?.FooterInfo ?? "版权所有";
            
            // 设置SEO信息
            ViewData["SeoTitle"] = "页面未找到 - 404";
            ViewData["SeoDescription"] = "您访问的页面不存在或已被删除";
            
            return View();
        }
    }
}