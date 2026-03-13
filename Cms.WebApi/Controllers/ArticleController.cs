using Cms.Application.DTOs;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null, int? channelId = null, int websiteId = 1)
        {
            var articles = await _articleService.GetListAsync(page, pageSize, keyword, channelId, websiteId);
            return Ok(articles);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ArticleDto articleDto)
        {
            var article = await _articleService.CreateAsync(articleDto);
            return CreatedAtAction(nameof(GetById), new { id = article.Id }, article);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ArticleDto articleDto)
        {
            if (id != articleDto.Id)
            {
                return BadRequest();
            }
            var updatedArticle = await _articleService.UpdateAsync(articleDto);
            return Ok(updatedArticle);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _articleService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            await _articleService.PublishAsync(id);
            return Ok();
        }

        [HttpPost("{id}/unpublish")]
        public async Task<IActionResult> Unpublish(int id)
        {
            await _articleService.UnpublishAsync(id);
            return Ok();
        }

        [HttpPost("{id}/view")]
        public async Task<IActionResult> IncrementViewCount(int id)
        {
            await _articleService.IncrementViewCountAsync(id);
            return Ok();
        }

        [HttpGet("headline/{websiteId}")]
        public async Task<IActionResult> GetHeadlineArticles(int websiteId, int limit = 5)
        {
            var articles = await _articleService.GetHeadlineArticlesAsync(websiteId, limit);
            return Ok(articles);
        }

        [HttpGet("hot/{websiteId}")]
        public async Task<IActionResult> GetHotArticles(int websiteId, int limit = 10)
        {
            var articles = await _articleService.GetHotArticlesAsync(websiteId, limit);
            return Ok(articles);
        }
    }
}