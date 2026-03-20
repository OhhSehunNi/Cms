using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 文章管理控制器
    /// 提供文章的CRUD操作、发布/下线、浏览量统计以及获取头条和热门文章等功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ArticleController : ControllerBase
    {
        /// <summary>
        /// 文章服务接口
        /// </summary>
        private readonly IArticleService _articleService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="articleService">文章服务实例</param>
        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        /// <summary>
        /// 根据ID获取文章信息
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <returns>文章信息</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="channelId">频道ID</param>
        /// <param name="status">状态</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="isTop">是否置顶</param>
        /// <param name="isRecommended">是否推荐</param>
        /// <param name="websiteId">网站ID，默认1</param>
        /// <returns>文章列表</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null, int? channelId = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null, bool? isTop = null, bool? isRecommended = null, int websiteId = 1)
        {
            var articles = await _articleService.GetListAsync(page, pageSize, keyword, channelId, status, startDate, endDate, isTop, isRecommended, websiteId);
            return Ok(articles);
        }

        /// <summary>
        /// 创建文章
        /// </summary>
        /// <param name="articleDto">文章信息</param>
        /// <returns>创建的文章信息</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ArticleDto articleDto)
        {
            try
            {
                var article = await _articleService.CreateAsync(articleDto);
                return CreatedAtAction(nameof(GetById), new { id = article.Id }, article);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 更新文章信息
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <param name="articleDto">文章信息</param>
        /// <returns>更新后的文章信息</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ArticleDto articleDto)
        {
            if (id != articleDto.Id)
            {
                return BadRequest();
            }
            try
            {
                var updatedArticle = await _articleService.UpdateAsync(articleDto);
                return Ok(updatedArticle);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _articleService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 发布文章
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <returns>发布后的文章信息</returns>
        [HttpPost("publish")]
        public async Task<IActionResult> Publish([FromBody] int id)
        {
            try
            {
                var article = await _articleService.PublishAsync(id);
                return Ok(article);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 下线文章
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <returns>下线后的文章信息</returns>
        [HttpPost("offline")]
        public async Task<IActionResult> Offline([FromBody] int id)
        {
            try
            {
                var article = await _articleService.OfflineAsync(id);
                return Ok(article);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 增加文章浏览量
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <returns>增加浏览量结果</returns>
        [HttpPost("{id}/view")]
        [AllowAnonymous]
        public async Task<IActionResult> IncrementViewCount(int id)
        {
            await _articleService.IncrementViewCountAsync(id);
            return Ok();
        }

        /// <summary>
        /// 获取头条文章
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <param name="limit">返回数量，默认5</param>
        /// <returns>头条文章列表</returns>
        [HttpGet("headline/{websiteId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHeadlineArticles(int websiteId, int limit = 5)
        {
            var articles = await _articleService.GetHeadlineArticlesAsync(websiteId, limit);
            return Ok(articles);
        }

        /// <summary>
        /// 获取热门文章
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <param name="limit">返回数量，默认10</param>
        /// <returns>热门文章列表</returns>
        [HttpGet("hot/{websiteId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotArticles(int websiteId, int limit = 10)
        {
            var articles = await _articleService.GetHotArticlesAsync(websiteId, limit);
            return Ok(articles);
        }
    }
}