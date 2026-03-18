using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 网站管理控制器
    /// 提供网站的CRUD操作以及根据域名获取网站信息等功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class WebsiteController : ControllerBase
    {
        /// <summary>
        /// 网站服务接口
        /// </summary>
        private readonly IWebsiteService _websiteService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="websiteService">网站服务实例</param>
        public WebsiteController(IWebsiteService websiteService)
        {
            _websiteService = websiteService;
        }

        /// <summary>
        /// 根据ID获取网站信息
        /// </summary>
        /// <param name="id">网站ID</param>
        /// <returns>网站信息</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var website = await _websiteService.GetByIdAsync(id);
            if (website == null)
            {
                return NotFound();
            }
            return Ok(website);
        }

        /// <summary>
        /// 根据域名获取网站信息
        /// </summary>
        /// <param name="domain">网站域名</param>
        /// <returns>网站信息</returns>
        [HttpGet("domain/{domain}")]
        public async Task<IActionResult> GetByDomain(string domain)
        {
            var website = await _websiteService.GetByDomainAsync(domain);
            if (website == null)
            {
                return NotFound();
            }
            return Ok(website);
        }

        /// <summary>
        /// 获取网站列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>网站列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null)
        {
            var websites = await _websiteService.GetListAsync(page, pageSize, keyword);
            return Ok(websites);
        }

        /// <summary>
        /// 创建网站
        /// </summary>
        /// <param name="websiteDto">网站信息</param>
        /// <returns>创建的网站信息</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WebsiteDto websiteDto)
        {
            var website = await _websiteService.CreateAsync(websiteDto);
            return CreatedAtAction(nameof(GetById), new { id = website.Id }, website);
        }

        /// <summary>
        /// 更新网站信息
        /// </summary>
        /// <param name="id">网站ID</param>
        /// <param name="websiteDto">网站信息</param>
        /// <returns>更新后的网站信息</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WebsiteDto websiteDto)
        {
            if (id != websiteDto.Id)
            {
                return BadRequest();
            }
            var updatedWebsite = await _websiteService.UpdateAsync(websiteDto);
            if (updatedWebsite == null)
            {
                return NotFound();
            }
            return Ok(updatedWebsite);
        }

        /// <summary>
        /// 删除网站
        /// </summary>
        /// <param name="id">网站ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _websiteService.DeleteAsync(id);
            return NoContent();
        }
    }
}