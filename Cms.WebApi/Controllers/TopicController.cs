using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 专题管理控制器
    /// 提供专题的CRUD操作、根据slug获取专题、获取专题文章以及为专题添加/移除文章等功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TopicController : ControllerBase
    {
        /// <summary>
        /// 专题服务接口
        /// </summary>
        private readonly ITopicService _topicService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="topicService">专题服务实例</param>
        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        /// <summary>
        /// 根据ID获取专题信息
        /// </summary>
        /// <param name="id">专题ID</param>
        /// <returns>专题信息</returns>
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

        /// <summary>
        /// 根据slug获取专题信息
        /// </summary>
        /// <param name="slug">专题slug</param>
        /// <param name="websiteId">网站ID，默认1</param>
        /// <returns>专题信息</returns>
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

        /// <summary>
        /// 获取专题列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="websiteId">网站ID，默认1</param>
        /// <returns>专题列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null, int websiteId = 1)
        {
            var topics = await _topicService.GetListAsync(page, pageSize, keyword, websiteId);
            return Ok(topics);
        }

        /// <summary>
        /// 创建专题
        /// </summary>
        /// <param name="topicDto">专题信息</param>
        /// <returns>创建的专题信息</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TopicDto topicDto)
        {
            var topic = await _topicService.CreateAsync(topicDto);
            return CreatedAtAction(nameof(GetById), new { id = topic.Id }, topic);
        }

        /// <summary>
        /// 更新专题信息
        /// </summary>
        /// <param name="id">专题ID</param>
        /// <param name="topicDto">专题信息</param>
        /// <returns>更新后的专题信息</returns>
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

        /// <summary>
        /// 删除专题
        /// </summary>
        /// <param name="id">专题ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _topicService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// 获取专题文章
        /// </summary>
        /// <param name="id">专题ID</param>
        /// <param name="websiteId">网站ID，默认1</param>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <returns>专题文章列表</returns>
        [HttpGet("{id}/articles")]
        public async Task<IActionResult> GetTopicArticles(int id, int websiteId = 1, int page = 1, int pageSize = 10)
        {
            var articles = await _topicService.GetTopicArticlesAsync(id, websiteId, page, pageSize);
            return Ok(articles);
        }

        /// <summary>
        /// 为专题添加文章
        /// </summary>
        /// <param name="id">专题ID</param>
        /// <param name="articleIds">文章ID列表</param>
        /// <returns>添加结果</returns>
        [HttpPost("{id}/articles")]
        public async Task<IActionResult> AddArticlesToTopic(int id, [FromBody] List<int> articleIds)
        {
            await _topicService.AddArticlesToTopicAsync(id, articleIds);
            return Ok();
        }

        /// <summary>
        /// 从专题中移除所有文章
        /// </summary>
        /// <param name="id">专题ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("{id}/articles")]
        public async Task<IActionResult> RemoveArticlesFromTopic(int id)
        {
            await _topicService.RemoveArticlesFromTopicAsync(id);
            return NoContent();
        }
    }
}