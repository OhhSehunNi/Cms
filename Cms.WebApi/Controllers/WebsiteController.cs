using Cms.Application.DTOs;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebsiteController : ControllerBase
    {
        private readonly IWebsiteService _websiteService;

        public WebsiteController(IWebsiteService websiteService)
        {
            _websiteService = websiteService;
        }

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

        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null)
        {
            var websites = await _websiteService.GetListAsync(page, pageSize, keyword);
            return Ok(websites);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WebsiteDto websiteDto)
        {
            var website = await _websiteService.CreateAsync(websiteDto);
            return CreatedAtAction(nameof(GetById), new { id = website.Id }, website);
        }

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _websiteService.DeleteAsync(id);
            return NoContent();
        }
    }
}