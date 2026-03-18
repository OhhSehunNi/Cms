using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 标签管理控制器
    /// 提供标签的CRUD操作、获取文章标签、为文章添加/移除标签以及获取带使用次数的标签等功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        /// <summary>
        /// 标签服务接口
        /// </summary>
        private readonly ITagService _tagService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tagService">标签服务实例</param>
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// 根据ID获取标签信息
        /// </summary>
        /// <param name="id">标签ID</param>
        /// <returns>标签信息</returns>
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

        /// <summary>
        /// 获取标签列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="websiteId">网站ID，默认1</param>
        /// <returns>标签列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null, int websiteId = 1)
        {
            var tags = await _tagService.GetListAsync(page, pageSize, keyword, websiteId);
            return Ok(tags);
        }

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="tagDto">标签信息</param>
        /// <returns>创建的标签信息</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TagDto tagDto)
        {
            var tag = await _tagService.CreateAsync(tagDto);
            return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
        }

        /// <summary>
        /// 更新标签信息
        /// </summary>
        /// <param name="id">标签ID</param>
        /// <param name="tagDto">标签信息</param>
        /// <returns>更新后的标签信息</returns>
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

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="id">标签ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _tagService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>所有标签列表</returns>
        [HttpGet("all/{websiteId}")]
        public async Task<IActionResult> GetAllTags(int websiteId)
        {
            var tags = await _tagService.GetAllTagsAsync(websiteId);
            return Ok(tags);
        }

        /// <summary>
        /// 获取文章标签
        /// </summary>
        /// <param name="articleId">文章ID</param>
        /// <returns>文章标签列表</returns>
        [HttpGet("article/{articleId}")]
        public async Task<IActionResult> GetArticleTags(int articleId)
        {
            var tags = await _tagService.GetArticleTagsAsync(articleId);
            return Ok(tags);
        }

        /// <summary>
        /// 为文章添加标签
        /// </summary>
        /// <param name="articleId">文章ID</param>
        /// <param name="tags">标签列表</param>
        /// <returns>添加结果</returns>
        [HttpPost("article/{articleId}")]
        public async Task<IActionResult> AddTagsToArticle(int articleId, [FromBody] List<string> tags)
        {
            await _tagService.AddTagsToArticleAsync(articleId, tags);
            return Ok();
        }

        /// <summary>
        /// 从文章中移除所有标签
        /// </summary>
        /// <param name="articleId">文章ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("article/{articleId}")]
        public async Task<IActionResult> RemoveTagsFromArticle(int articleId)
        {
            await _tagService.RemoveTagsFromArticleAsync(articleId);
            return NoContent();
        }

        /// <summary>
        /// 获取带使用次数的标签
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <param name="limit">返回数量，默认20</param>
        /// <returns>带使用次数的标签列表</returns>
        [HttpGet("count/{websiteId}")]
        public async Task<IActionResult> GetTagsWithCount(int websiteId, int limit = 20)
        {
            var tagsWithCount = await _tagService.GetTagsWithCountAsync(websiteId, limit);
            return Ok(tagsWithCount);
        }
    }
}