using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SEOResourceController : ControllerBase
    {
        private readonly ISEOService _seoService;

        public SEOResourceController(ISEOService seoService)
        {
            _seoService = seoService;
        }

        [HttpGet("sitemap")]
        public async Task<IActionResult> GetSitemap()
        {
            var sitemap = await _seoService.GenerateSitemapAsync();
            return Content(sitemap, "application/xml");
        }

        [HttpGet("robots")]
        public async Task<IActionResult> GetRobotsTxt()
        {
            var robotsTxt = await _seoService.GenerateRobotsTxtAsync();
            return Content(robotsTxt, "text/plain");
        }

        [HttpGet("breadcrumbs")]
        public async Task<IActionResult> GetBreadcrumbs(int? channelId = null, int? articleId = null)
        {
            var breadcrumbs = await _seoService.GenerateBreadcrumbsAsync(channelId, articleId);
            return Ok(new { breadcrumbs });
        }
    }
}