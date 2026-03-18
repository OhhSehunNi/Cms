using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cms.Application.Services
{
    /// <summary>
    /// 角色栏目服务，用于管理角色与栏目之间的关联关系
    /// </summary>
    public class RoleChannelService : IRoleChannelService
    {
        private readonly CmsDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public RoleChannelService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 为角色添加栏目权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="channelIds">栏目ID列表</param>
        /// <returns></returns>
        public async Task AddChannelsToRoleAsync(int roleId, List<int> channelIds)
        {
            // 删除现有的角色栏目关联
            var existingRoleChannels = await _dbContext.CmsRoleChannels
                .Where(rc => rc.RoleId == roleId)
                .ToListAsync();

            _dbContext.CmsRoleChannels.RemoveRange(existingRoleChannels);

            // 添加新的角色栏目关联
            foreach (var channelId in channelIds)
            {
                var roleChannel = new CmsRoleChannel
                {
                    RoleId = roleId,
                    ChannelId = channelId
                };
                _dbContext.CmsRoleChannels.Add(roleChannel);
            }

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 获取角色的栏目权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>栏目ID列表</returns>
        public async Task<List<int>> GetRoleChannelsAsync(int roleId)
        {
            return await _dbContext.CmsRoleChannels
                .Where(rc => rc.RoleId == roleId)
                .Select(rc => rc.ChannelId)
                .ToListAsync();
        }

        /// <summary>
        /// 删除角色的栏目权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="channelId">栏目ID</param>
        /// <returns></returns>
        public async Task RemoveChannelFromRoleAsync(int roleId, int channelId)
        {
            var roleChannel = await _dbContext.CmsRoleChannels
                .FirstOrDefaultAsync(rc => rc.RoleId == roleId && rc.ChannelId == channelId);

            if (roleChannel != null)
            {
                _dbContext.CmsRoleChannels.Remove(roleChannel);
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 删除角色的所有栏目权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        public async Task RemoveAllChannelsFromRoleAsync(int roleId)
        {
            var roleChannels = await _dbContext.CmsRoleChannels
                .Where(rc => rc.RoleId == roleId)
                .ToListAsync();

            _dbContext.CmsRoleChannels.RemoveRange(roleChannels);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 检查角色是否有栏目权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="channelId">栏目ID</param>
        /// <returns>是否有权限</returns>
        public async Task<bool> HasChannelPermissionAsync(int roleId, int channelId)
        {
            return await _dbContext.CmsRoleChannels
                .AnyAsync(rc => rc.RoleId == roleId && rc.ChannelId == channelId);
        }
    }
}