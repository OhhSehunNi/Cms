using Microsoft.AspNetCore.Mvc;
using Cms.Application.Services;

namespace Cms.Web.Controllers
{
    /// <summary>
    /// 错误控制器
    /// 处理网站错误页面的展示
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// 网站服务接口
        /// </summary>
        private readonly IWebsiteService _websiteService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="websiteService">网站服务实例</param>
        public ErrorController(IWebsiteService websiteService)
        {
            _websiteService = websiteService;
        }

        /// <summary>
        /// 404错误页面
        /// </summary>
        /// <returns>404错误视图</returns>
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