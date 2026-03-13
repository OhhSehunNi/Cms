using Cms.Application.DTOs;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var topic = await _topicService.GetByIdAsync(id);
            if (topic == null)
            {
                return NotFound();
            }
            return Ok(topic);
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug, int websiteId = 1)
        {
            var topic = await _topicService.GetBySlugAsync(slug, websiteId);
            if (topic == null)
            {
                return NotFound();
            }
            return Ok(topic);
        }

        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null, int websiteId = 1)
        {
            var topics = await _topicService.GetListAsync(page, pageSize, keyword, websiteId);
            return Ok(topics);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TopicDto topicDto)
        {
            var topic = await _topicService.CreateAsync(topicDto);
            return CreatedAtAction(nameof(GetById), new { id = topic.Id }, topic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TopicDto topicDto)
        {
            if (id != topicDto.Id)
            {
                return BadRequest();
            }
            var updatedTopic = await _topicService.UpdateAsync(topicDto);
            return Ok(updatedTopic);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _topicService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}/articles")]
        public async Task<IActionResult> GetTopicArticles(int id, int websiteId = 1, int page = 1, int pageSize = 10)
        {
            var articles = await _topicService.GetTopicArticlesAsync(id, websiteId, page, pageSize);
            return Ok(articles);
        }

        [HttpPost("{id}/articles")]
        public async Task<IActionResult> AddArticlesToTopic(int id, [FromBody] List<int> articleIds)
        {
            await _topicService.AddArticlesToTopicAsync(id, articleIds);
            return Ok();
        }

        [HttpDelete("{id}/articles")]
        public async Task<IActionResult> RemoveArticlesFromTopic(int id)
        {
            await _topicService.RemoveArticlesFromTopicAsync(id);
            return NoContent();
        }
    }
}