using Cms.Application.DTOs;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return Ok(tag);
        }

        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null, int websiteId = 1)
        {
            var tags = await _tagService.GetListAsync(page, pageSize, keyword, websiteId);
            return Ok(tags);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TagDto tagDto)
        {
            var tag = await _tagService.CreateAsync(tagDto);
            return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TagDto tagDto)
        {
            if (id != tagDto.Id)
            {
                return BadRequest();
            }
            var updatedTag = await _tagService.UpdateAsync(tagDto);
            return Ok(updatedTag);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _tagService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("all/{websiteId}")]
        public async Task<IActionResult> GetAllTags(int websiteId)
        {
            var tags = await _tagService.GetAllTagsAsync(websiteId);
            return Ok(tags);
        }

        [HttpGet("article/{articleId}")]
        public async Task<IActionResult> GetArticleTags(int articleId)
        {
            var tags = await _tagService.GetArticleTagsAsync(articleId);
            return Ok(tags);
        }

        [HttpPost("article/{articleId}")]
        public async Task<IActionResult> AddTagsToArticle(int articleId, [FromBody] List<string> tags)
        {
            await _tagService.AddTagsToArticleAsync(articleId, tags);
            return Ok();
        }

        [HttpDelete("article/{articleId}")]
        public async Task<IActionResult> RemoveTagsFromArticle(int articleId)
        {
            await _tagService.RemoveTagsFromArticleAsync(articleId);
            return NoContent();
        }

        [HttpGet("count/{websiteId}")]
        public async Task<IActionResult> GetTagsWithCount(int websiteId, int limit = 20)
        {
            var tagsWithCount = await _tagService.GetTagsWithCountAsync(websiteId, limit);
            return Ok(tagsWithCount);
        }
    }
}