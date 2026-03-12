using Cms.Application.DTOs;

namespace Cms.Application.Services
{
    public interface IChannelService
    {
        Task<ChannelDto> GetByIdAsync(int id);
        Task<List<ChannelDto>> GetTreeAsync();
        Task<List<ChannelDto>> GetListAsync(int page, int pageSize, string? keyword = null);
        Task<ChannelDto> CreateAsync(ChannelDto channelDto);
        Task<ChannelDto> UpdateAsync(ChannelDto channelDto);
        Task DeleteAsync(int id);
        Task<List<ChannelDto>> GetNavigationChannelsAsync();
    }
}