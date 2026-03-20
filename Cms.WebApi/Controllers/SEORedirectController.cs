using Cms.Application.Services;
using Cms.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// SEO管理控制器
    /// 提供SEO设置、站点地图、robots.txt和重定向规则管理功能
    /// </summary>
    [Route("api/seo")]
    [ApiController]
    [Authorize]
    public class SEORedirectController : ControllerBase
    {
        /// <summary>
        /// SEO服务接口
        /// </summary>
        private readonly ISEOService _seoService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="seoService">SEO服务实例</param>
        public SEORedirectController(ISEOService seoService)
        {
            _seoService = seoService;
        }

        /// <summary>
        /// 获取SEO设置
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>SEO设置</returns>
        [HttpGet("settings")]
        public async Task<IActionResult> GetSEOSettings(int websiteId = 1)
        {
            var settings = await _seoService.GetSEOSettingsAsync(websiteId);
            if (settings == null)
            {
                return NotFound();
            }
            return Ok(settings);
        }

        /// <summary>
        /// 更新SEO设置
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <param name="settings">SEO设置</param>
        /// <returns>是否更新成功</returns>
        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSEOSettings(int websiteId = 1, [FromBody] object settings = null)
        {
            var result = await _seoService.UpdateSEOSettingsAsync(websiteId, settings);
            if (!result)
            {
                return BadRequest("更新SEO设置失败");
            }
            return Ok(new { success = true, message = "SEO设置更新成功" });
        }

        /// <summary>
        /// 获取站点地图
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>站点地图XML</returns>
        [HttpGet("sitemap")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSitemap(int websiteId = 1)
        {
            var sitemap = await _seoService.GenerateSitemapAsync(websiteId);
            return Content(sitemap, "application/xml");
        }

        /// <summary>
        /// 获取robots.txt内容
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>robots.txt内容</returns>
        [HttpGet("robots")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRobotsTxt(int websiteId = 1)
        {
            var robotsTxt = await _seoService.GenerateRobotsTxtAsync(websiteId);
            return Content(robotsTxt, "text/plain");
        }

        /// <summary>
        /// 添加重定向规则
        /// </summary>
        /// <param name="redirect">重定向规则</param>
        /// <returns>重定向规则ID</returns>
        [HttpPost("redirects")]
        public async Task<IActionResult> AddRedirect([FromBody] CmsSeoRedirect redirect)
        {
            try
            {
                var id = await _seoService.AddRedirectAsync(redirect);
                return CreatedAtAction(nameof(GetRedirects), new { websiteId = redirect.WebsiteId }, new { id, success = true, message = "重定向规则添加成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 获取重定向规则列表
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>重定向规则列表</returns>
        [HttpGet("redirects")]
        public async Task<IActionResult> GetRedirects(int websiteId = 1)
        {
            var redirects = await _seoService.GetRedirectsAsync(websiteId);
            return Ok(redirects);
        }

        /// <summary>
        /// 删除重定向规则
        /// </summary>
        /// <param name="id">重定向规则ID</param>
        /// <returns>是否删除成功</returns>
        [HttpDelete("redirects/{id}")]
        public async Task<IActionResult> DeleteRedirect(int id)
        {
            try
            {
                var result = await _seoService.DeleteRedirectAsync(id);
                if (!result)
                {
                    return NotFound("重定向规则不存在");
                }
                return Ok(new { success = true, message = "重定向规则删除成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 生成Slug
        /// </summary>
        /// <param name="title">标题</param>
        /// <returns>Slug字符串</returns>
        [HttpGet("slug")]
        [AllowAnonymous]
        public IActionResult GenerateSlug([FromQuery] string title)
        {
            var slug = _seoService.GenerateSlug(title);
            return Ok(new { slug });
        }
    }
}