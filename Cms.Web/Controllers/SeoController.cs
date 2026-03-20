using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers
{
    /// <summary>
    /// SEO控制器
    /// 处理网站SEO相关功能，如生成网站地图和robots.txt文件
    /// </summary>
    public class SeoController : Controller
    {
        /// <summary>
        /// SEO服务接口
        /// </summary>
        private readonly ISEOService _seoService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="seoService">SEO服务实例</param>
        public SeoController(ISEOService seoService)
        {
            _seoService = seoService;
        }

        /// <summary>
        /// 生成网站地图
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>网站地图XML内容</returns>
        [HttpGet("/sitemap.xml")]
        public async Task<IActionResult> Sitemap(int websiteId = 1)
        {
            var sitemap = await _seoService.GenerateSitemapAsync(websiteId);
            return Content(sitemap, "application/xml");
        }

        /// <summary>
        /// 生成robots.txt文件
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>robots.txt文本内容</returns>
        [HttpGet("/robots.txt")]
        public async Task<IActionResult> RobotsTxt(int websiteId = 1)
        {
            var robotsTxt = await _seoService.GenerateRobotsTxtAsync(websiteId);
            return Content(robotsTxt, "text/plain");
        }
    }
}