using Cms.Application.Services.Dtos;

namespace Cms.Application.Services
{
    /// <summary>
    /// 权限服务接口，用于权限相关的业务逻辑
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// 根据 ID 获取权限
        /// </summary>
        /// <param name="id">权限 ID</param>
        /// <returns>权限 DTO</returns>
        Task<PermissionDto> GetByIdAsync(int id);

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <returns>权限 DTO 列表</returns>
        Task<List<PermissionDto>> GetListAsync(int page, int pageSize, string? keyword = null);

        /// <summary>
        /// 创建权限
        /// </summary>
        /// <param name="permissionDto">权限 DTO</param>
        /// <returns>创建后的权限 DTO</returns>
        Task<PermissionDto> CreateAsync(PermissionDto permissionDto);

        /// <summary>
        /// 更新权限
        /// </summary>
        /// <param name="permissionDto">权限 DTO</param>
        /// <returns>更新后的权限 DTO</returns>
        Task<PermissionDto> UpdateAsync(PermissionDto permissionDto);

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id">权限 ID</param>
        /// <returns></returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// 获取权限总数
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns>权限总数</returns>
        Task<int> GetCountAsync(string? keyword = null);

        /// <summary>
        /// 获取权限分类列表
        /// </summary>
        /// <returns>权限分类列表</returns>
        Task<List<string>> GetCategoriesAsync();
    }
}