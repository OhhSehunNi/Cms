using Cms.Application.Services.Dtos;

namespace Cms.Application.Services
{
    /// <summary>
    /// 用户服务接口，用于用户相关的业务逻辑
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 根据 ID 获取用户
        /// </summary>
        /// <param name="id">用户 ID</param>
        /// <returns>用户 DTO</returns>
        Task<UserDto> GetByIdAsync(int id);

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户 DTO</returns>
        Task<UserDto> GetByUsernameAsync(string username);

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <returns>用户 DTO 列表</returns>
        Task<List<UserDto>> GetListAsync(int page, int pageSize, string? keyword = null);

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="registerDto">注册 DTO</param>
        /// <returns>创建后的用户 DTO</returns>
        Task<UserDto> CreateAsync(RegisterDto registerDto);

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="userDto">用户 DTO</param>
        /// <returns>更新后的用户 DTO</returns>
        Task<UserDto> UpdateAsync(UserDto userDto);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户 ID</param>
        /// <returns></returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// 验证用户凭证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>是否验证成功</returns>
        Task<bool> ValidateCredentialsAsync(string username, string password);

        /// <summary>
        /// 获取用户角色
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>角色列表</returns>
        Task<List<string>> GetUserRolesAsync(int userId);

        /// <summary>
        /// 检查用户权限
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <param name="permissionCode">权限代码</param>
        /// <returns>是否有权限</returns>
        Task<bool> CheckPermissionAsync(int userId, string permissionCode);

        /// <summary>
        /// 生成BCrypt密码
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <returns>BCrypt加密后的密码</returns>
        string GenerateBCryptPassword(string password);

        /// <summary>
        /// 分配用户角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleIds">角色ID列表</param>
        /// <returns></returns>
        Task AssignRolesAsync(int userId, List<int> roleIds);

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        Task ResetPasswordAsync(int userId, string newPassword);

        /// <summary>
        /// 获取用户总数
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns>用户总数</returns>
        Task<int> GetCountAsync(string? keyword = null);
    }
}