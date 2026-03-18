using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 推荐管理控制器
    /// 提供推荐位和推荐项的CRUD操作，以及获取推荐文章等功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendController : ControllerBase
    {
        /// <summary>
        /// 推荐服务接口
        /// </summary>
        private readonly IRecommendService _recommendService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="recommendService">推荐服务实例</param>
        public RecommendController(IRecommendService recommendService)
        {
            _recommendService = recommendService;
        }

        // 推荐位相关接口
        /// <summary>
        /// 根据ID获取推荐位信息
        /// </summary>
        /// <param name="id">推荐位ID</param>
        /// <returns>推荐位信息</returns>
        [HttpGet("slot/{id}")]
        public async Task<IActionResult> GetSlotById(int id)
        {
            var slot = await _recommendService.GetSlotByIdAsync(id);
            if (slot == null)
            {
                return NotFound();
            }
            return Ok(slot);
        }

        /// <summary>
        /// 根据代码获取推荐位信息
        /// </summary>
        /// <param name="code">推荐位代码</param>
        /// <returns>推荐位信息</returns>
        [HttpGet("slot/code/{code}")]
        public async Task<IActionResult> GetSlotByCode(string code)
        {
            var slot = await _recommendService.GetSlotByCodeAsync(code);
            if (slot == null)
            {
                return NotFound();
            }
            return Ok(slot);
        }

        /// <summary>
        /// 获取推荐位列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>推荐位列表</returns>
        [HttpGet("slots")]
        public async Task<IActionResult> GetSlotList(int page = 1, int pageSize = 10, string? keyword = null)
        {
            var slots = await _recommendService.GetSlotListAsync(page, pageSize, keyword);
            return Ok(slots);
        }

        /// <summary>
        /// 创建推荐位
        /// </summary>
        /// <param name="slotDto">推荐位信息</param>
        /// <returns>创建的推荐位信息</returns>
        [HttpPost("slot")]
        public async Task<IActionResult> CreateSlot([FromBody] RecommendSlotDto slotDto)
        {
            var slot = await _recommendService.CreateSlotAsync(slotDto);
            return CreatedAtAction(nameof(GetSlotById), new { id = slot.Id }, slot);
        }

        /// <summary>
        /// 更新推荐位信息
        /// </summary>
        /// <param name="id">推荐位ID</param>
        /// <param name="slotDto">推荐位信息</param>
        /// <returns>更新后的推荐位信息</returns>
        [HttpPut("slot/{id}")]
        public async Task<IActionResult> UpdateSlot(int id, [FromBody] RecommendSlotDto slotDto)
        {
            if (id != slotDto.Id)
            {
                return BadRequest();
            }
            var updatedSlot = await _recommendService.UpdateSlotAsync(slotDto);
            return Ok(updatedSlot);
        }

        /// <summary>
        /// 删除推荐位
        /// </summary>
        /// <param name="id">推荐位ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("slot/{id}")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            await _recommendService.DeleteSlotAsync(id);
            return NoContent();
        }

        // 推荐项相关接口
        /// <summary>
        /// 添加推荐项
        /// </summary>
        /// <param name="itemDto">推荐项信息</param>
        /// <returns>添加的推荐项信息</returns>
        [HttpPost("item")]
        public async Task<IActionResult> AddItem([FromBody] RecommendItemDto itemDto)
        {
            var item = await _recommendService.AddItemAsync(itemDto);
            return Ok(item);
        }

        /// <summary>
        /// 更新推荐项信息
        /// </summary>
        /// <param name="id">推荐项ID</param>
        /// <param name="itemDto">推荐项信息</param>
        /// <returns>更新后的推荐项信息</returns>
        [HttpPut("item/{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] RecommendItemDto itemDto)
        {
            if (id != itemDto.Id)
            {
                return BadRequest();
            }
            var updatedItem = await _recommendService.UpdateItemAsync(itemDto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// 删除推荐项
        /// </summary>
        /// <param name="id">推荐项ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("item/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _recommendService.DeleteItemAsync(id);
            return NoContent();
        }

        // 获取推荐文章
        /// <summary>
        /// 获取推荐文章
        /// </summary>
        /// <param name="code">推荐位代码</param>
        /// <param name="count">返回数量，默认10</param>
        /// <returns>推荐文章列表</returns>
        [HttpGet("articles/{code}")]
        public async Task<IActionResult> GetRecommendArticles(string code, int count = 10)
        {
            var articles = await _recommendService.GetRecommendArticlesAsync(code, count);
            return Ok(articles);
        }
    }
}