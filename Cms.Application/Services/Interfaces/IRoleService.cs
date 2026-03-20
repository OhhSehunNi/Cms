using Cms.Application.Services.Dtos;

namespace Cms.Application.Services
{
    /// <summary>
    /// 角色服务接口，用于角色相关的业务逻辑
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// 根据 ID 获取角色
        /// </summary>
        /// <param name="id">角色 ID</param>
        /// <returns>角色 DTO</returns>
        Task<RoleDto> GetByIdAsync(int id);

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <returns>角色 DTO 列表</returns>
        Task<List<RoleDto>> GetListAsync(int page, int pageSize, string? keyword = null);

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="roleDto">角色 DTO</param>
        /// <returns>创建后的角色 DTO</returns>
        Task<RoleDto> CreateAsync(RoleDto roleDto);

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="roleDto">角色 DTO</param>
        /// <returns>更新后的角色 DTO</returns>
        Task<RoleDto> UpdateAsync(RoleDto roleDto);

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色 ID</param>
        /// <returns></returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="roleId">角色 ID</param>
        /// <returns>权限 DTO 列表</returns>
        Task<List<PermissionDto>> GetRolePermissionsAsync(int roleId);

        /// <summary>
        /// 更新角色权限
        /// </summary>
        /// <param name="roleId">角色 ID</param>
        /// <param name="permissionIds">权限 ID 列表</param>
        /// <returns></returns>
        Task UpdateRolePermissionsAsync(int roleId, List<int> permissionIds);

        /// <summary>
        /// 获取角色可管理的栏目
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>栏目ID列表</returns>
        Task<List<int>> GetRoleChannelsAsync(int roleId);

        /// <summary>
        /// 更新角色栏目权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="channelIds">栏目ID列表</param>
        /// <returns></returns>
        Task UpdateRoleChannelsAsync(int roleId, List<int> channelIds);

        /// <summary>
        /// 获取角色总数
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns>角色总数</returns>
        Task<int> GetCountAsync(string? keyword = null);
    }
}