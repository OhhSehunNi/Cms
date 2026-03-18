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
        /// <returns>网站地图XML内容</returns>
        [HttpGet("/sitemap.xml")]
        public async Task<IActionResult> Sitemap()
        {
            var sitemap = await _seoService.GenerateSitemapAsync();
            return Content(sitemap, "application/xml");
        }

        /// <summary>
        /// 生成robots.txt文件
        /// </summary>
        /// <returns>robots.txt文本内容</returns>
        [HttpGet("/robots.txt")]
        public async Task<IActionResult> RobotsTxt()
        {
            var robotsTxt = await _seoService.GenerateRobotsTxtAsync();
            return Content(robotsTxt, "text/plain");
        }
    }
}