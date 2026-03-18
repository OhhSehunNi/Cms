using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 频道管理控制器
    /// 提供频道的CRUD操作、获取频道树以及导航频道等功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ChannelController : ControllerBase
    {
        /// <summary>
        /// 频道服务接口
        /// </summary>
        private readonly IChannelService _channelService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="channelService">频道服务实例</param>
        public ChannelController(IChannelService channelService)
        {
            _channelService = channelService;
        }

        /// <summary>
        /// 根据ID获取频道信息
        /// </summary>
        /// <param name="id">频道ID</param>
        /// <returns>频道信息</returns>
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

        /// <summary>
        /// 获取频道树
        /// </summary>
        /// <returns>频道树结构</returns>
        [HttpGet("tree")]
        public async Task<IActionResult> GetTree()
        {
            var channels = await _channelService.GetTreeAsync();
            return Ok(channels);
        }

        /// <summary>
        /// 获取频道列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>频道列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null)
        {
            var channels = await _channelService.GetListAsync(page, pageSize, keyword);
            return Ok(channels);
        }

        /// <summary>
        /// 创建频道
        /// </summary>
        /// <param name="channelDto">频道信息</param>
        /// <returns>创建的频道信息</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChannelDto channelDto)
        {
            var channel = await _channelService.CreateAsync(channelDto);
            return CreatedAtAction(nameof(GetById), new { id = channel.Id }, channel);
        }

        /// <summary>
        /// 更新频道信息
        /// </summary>
        /// <param name="id">频道ID</param>
        /// <param name="channelDto">频道信息</param>
        /// <returns>更新后的频道信息</returns>
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

        /// <summary>
        /// 删除频道
        /// </summary>
        /// <param name="id">频道ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _channelService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// 获取导航频道
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>导航频道列表</returns>
        [HttpGet("navigation/{websiteId}")]
        public async Task<IActionResult> GetNavigationChannels(int websiteId)
        {
            var channels = await _channelService.GetNavigationChannelsAsync(websiteId);
            return Ok(channels);
        }
    }
}
