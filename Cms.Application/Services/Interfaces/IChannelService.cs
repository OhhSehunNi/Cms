using Cms.Application.Services.Dtos;

namespace Cms.Application.Services
{
    /// <summary>
    /// 栏目服务接口，用于栏目相关的业务逻辑
    /// </summary>
    public interface IChannelService
    {
        /// <summary>
        /// 根据 ID 获取栏目
        /// </summary>
        /// <param name="id">栏目 ID</param>
        /// <returns>栏目 DTO</returns>
        Task<ChannelDto> GetByIdAsync(int id);

        /// <summary>
        /// 获取栏目树
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>栏目 DTO 列表</returns>
        Task<List<ChannelDto>> GetTreeAsync(int websiteId);

        /// <summary>
        /// 获取栏目列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>栏目 DTO 列表</returns>
        Task<List<ChannelDto>> GetListAsync(int page, int pageSize, string? keyword = null, int websiteId = 1);

        /// <summary>
        /// 创建栏目
        /// </summary>
        /// <param name="channelDto">栏目 DTO</param>
        /// <returns>创建后的栏目 DTO</returns>
        Task<ChannelDto> CreateAsync(ChannelDto channelDto);

        /// <summary>
        /// 更新栏目
        /// </summary>
        /// <param name="channelDto">栏目 DTO</param>
        /// <returns>更新后的栏目 DTO</returns>
        Task<ChannelDto> UpdateAsync(ChannelDto channelDto);

        /// <summary>
        /// 删除栏目
        /// </summary>
        /// <param name="id">栏目 ID</param>
        /// <returns></returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// 获取导航栏目
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>栏目 DTO 列表</returns>
        Task<List<ChannelDto>> GetNavigationChannelsAsync(int websiteId);

        /// <summary>
        /// 修改排序
        /// </summary>
        /// <param name="sortRequests">排序请求</param>
        /// <returns></returns>
        Task UpdateSortAsync(List<SortRequestDto> sortRequests);

        /// <summary>
        /// 启用/停用栏目
        /// </summary>
        /// <param name="id">栏目 ID</param>
        /// <returns>更新后的栏目 DTO</returns>
        Task<ChannelDto> ToggleStatusAsync(int id);
    }
}