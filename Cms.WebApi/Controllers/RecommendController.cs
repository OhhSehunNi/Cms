using Cms.Application.DTOs;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendController : ControllerBase
    {
        private readonly IRecommendService _recommendService;

        public RecommendController(IRecommendService recommendService)
        {
            _recommendService = recommendService;
        }

        // 推荐位相关接口
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

        [HttpGet("slots")]
        public async Task<IActionResult> GetSlotList(int page = 1, int pageSize = 10, string? keyword = null)
        {
            var slots = await _recommendService.GetSlotListAsync(page, pageSize, keyword);
            return Ok(slots);
        }

        [HttpPost("slot")]
        public async Task<IActionResult> CreateSlot([FromBody] RecommendSlotDto slotDto)
        {
            var slot = await _recommendService.CreateSlotAsync(slotDto);
            return CreatedAtAction(nameof(GetSlotById), new { id = slot.Id }, slot);
        }

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

        [HttpDelete("slot/{id}")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            await _recommendService.DeleteSlotAsync(id);
            return NoContent();
        }

        // 推荐项相关接口
        [HttpPost("item")]
        public async Task<IActionResult> AddItem([FromBody] RecommendItemDto itemDto)
        {
            var item = await _recommendService.AddItemAsync(itemDto);
            return Ok(item);
        }

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

        [HttpDelete("item/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _recommendService.DeleteItemAsync(id);
            return NoContent();
        }

        // 获取推荐文章
        [HttpGet("articles/{code}")]
        public async Task<IActionResult> GetRecommendArticles(string code, int count = 10)
        {
            var articles = await _recommendService.GetRecommendArticlesAsync(code, count);
            return Ok(articles);
        }
    }
}