using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers
{
    public class SeoController : Controller
    {
        private readonly ISEOService _seoService;

        public SeoController(ISEOService seoService)
        {
            _seoService = seoService;
        }

        [HttpGet("/sitemap.xml")]
        public async Task<IActionResult> Sitemap()
        {
            var sitemap = await _seoService.GenerateSitemapAsync();
            return Content(sitemap, "application/xml");
        }

        [HttpGet("/robots.txt")]
        public async Task<IActionResult> RobotsTxt()
        {
            var robotsTxt = await _seoService.GenerateRobotsTxtAsync();
            return Content(robotsTxt, "text/plain");
        }
    }
}