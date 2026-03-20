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
            try
            {
                var channel = await _channelService.GetByIdAsync(id);
                if (channel == null)
                {
                    return NotFound("栏目不存在");
                }
                return Ok(channel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 获取频道树
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>频道树结构</returns>
        [HttpGet("tree")]
        public async Task<IActionResult> GetTree(int websiteId = 1)
        {
            try
            {
                var channels = await _channelService.GetTreeAsync(websiteId);
                return Ok(channels);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 获取频道列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="websiteId">网站ID，默认1</param>
        /// <returns>频道列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null, int websiteId = 1)
        {
            try
            {
                var channels = await _channelService.GetListAsync(page, pageSize, keyword, websiteId);
                return Ok(channels);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 创建频道
        /// </summary>
        /// <param name="channelDto">频道信息</param>
        /// <returns>创建的频道信息</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChannelDto channelDto)
        {
            try
            {
                var channel = await _channelService.CreateAsync(channelDto);
                return CreatedAtAction(nameof(GetById), new { id = channel.Id }, channel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
            try
            {
                if (id != channelDto.Id)
                {
                    return BadRequest("ID不匹配");
                }
                var updatedChannel = await _channelService.UpdateAsync(channelDto);
                return Ok(updatedChannel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 删除频道
        /// </summary>
        /// <param name="id">频道ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _channelService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 获取导航频道
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>导航频道列表</returns>
        [HttpGet("navigation/{websiteId}")]
        public async Task<IActionResult> GetNavigationChannels(int websiteId)
        {
            try
            {
                var channels = await _channelService.GetNavigationChannelsAsync(websiteId);
                return Ok(channels);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 修改排序
        /// </summary>
        /// <param name="sortRequests">排序请求</param>
        /// <returns>无内容</returns>
        [HttpPost("sort")]
        public async Task<IActionResult> UpdateSort([FromBody] List<SortRequestDto> sortRequests)
        {
            try
            {
                await _channelService.UpdateSortAsync(sortRequests);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 启用/停用栏目
        /// </summary>
        /// <param name="id">栏目ID</param>
        /// <returns>更新后的栏目信息</returns>
        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var channel = await _channelService.ToggleStatusAsync(id);
                return Ok(channel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
