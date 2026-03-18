using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cms.Application.Services
{
    /// <summary>
    /// 角色栏目服务接口
    /// </summary>
    public interface IRoleChannelService
    {
        Task AddChannelsToRoleAsync(int roleId, List<int> channelIds);
        Task<List<int>> GetRoleChannelsAsync(int roleId);
        Task RemoveChannelFromRoleAsync(int roleId, int channelId);
        Task RemoveAllChannelsFromRoleAsync(int roleId);
        Task<bool> HasChannelPermissionAsync(int roleId, int channelId);
    }
}