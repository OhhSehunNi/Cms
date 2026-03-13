using Cms.Application.DTOs;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChannelController : ControllerBase
    {
        private readonly IChannelService _channelService;

        public ChannelController(IChannelService channelService)
        {
            _channelService = channelService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var channel = await _channelService.GetByIdAsync(id);
            if (channel == null)
            {
                return NotFound();
            }
            return Ok(channel);
        }

        [HttpGet("tree")]
        public async Task<IActionResult> GetTree()
        {
            var channels = await _channelService.GetTreeAsync();
            return Ok(channels);
        }

        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null)
        {
            var channels = await _channelService.GetListAsync(page, pageSize, keyword);
            return Ok(channels);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChannelDto channelDto)
        {
            var channel = await _channelService.CreateAsync(channelDto);
            return CreatedAtAction(nameof(GetById), new { id = channel.Id }, channel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ChannelDto channelDto)
        {
            if (id != channelDto.Id)
            {
                return BadRequest();
            }
            var updatedChannel = await _channelService.UpdateAsync(channelDto);
            return Ok(updatedChannel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _channelService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("navigation/{websiteId}")]
        public async Task<IActionResult> GetNavigationChannels(int websiteId)
        {
            var channels = await _channelService.GetNavigationChannelsAsync(websiteId);
            return Ok(channels);
        }
    }
}