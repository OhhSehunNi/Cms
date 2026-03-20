using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// SEO资源控制器
    /// 提供站点地图、robots.txt文件和面包屑导航的生成功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SEOResourceController : ControllerBase
    {
        /// <summary>
        /// SEO服务接口
        /// </summary>
        private readonly ISEOService _seoService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="seoService">SEO服务实例</param>
        public SEOResourceController(ISEOService seoService)
        {
            _seoService = seoService;
        }

        /// <summary>
        /// 获取站点地图
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>XML格式的站点地图</returns>
        [HttpGet("sitemap")]
        public async Task<IActionResult> GetSitemap(int websiteId = 1)
        {
            var sitemap = await _seoService.GenerateSitemapAsync(websiteId);
            return Content(sitemap, "application/xml");
        }

        /// <summary>
        /// 获取robots.txt文件
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>文本格式的robots.txt内容</returns>
        [HttpGet("robots")]
        public async Task<IActionResult> GetRobotsTxt(int websiteId = 1)
        {
            var robotsTxt = await _seoService.GenerateRobotsTxtAsync(websiteId);
            return Content(robotsTxt, "text/plain");
        }

        /// <summary>
        /// 获取面包屑导航
        /// </summary>
        /// <param name="channelId">频道ID</param>
        /// <param name="articleId">文章ID</param>
        /// <param name="websiteId">网站ID</param>
        /// <returns>面包屑导航数据</returns>
        [HttpGet("breadcrumbs")]
        public async Task<IActionResult> GetBreadcrumbs(int? channelId = null, int? articleId = null, int? websiteId = null)
        {
            var breadcrumbs = await _seoService.GenerateBreadcrumbsAsync(channelId, articleId, websiteId);
            return Ok(new { breadcrumbs });
        }
    }
}