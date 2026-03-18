using Cms.Application.Services.Dtos;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Cms.Application.Services
{
    /// <summary>
    /// 用户服务实现类，用于用户相关的业务逻辑
    /// </summary>
    public class UserService : IUserService
    {
        private readonly CmsDbContext _dbContext;
        private readonly ICacheService _cacheService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="cacheService">缓存服务</param>
        public UserService(CmsDbContext dbContext, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }

        /// <summary>
        /// 根据 ID 获取用户
        /// </summary>
        /// <param name="id">用户 ID</param>
        /// <returns>用户 DTO</returns>
        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _dbContext.CmsUsers
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return null;

            return MapToDto(user);
        }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户 DTO</returns>
        public async Task<UserDto> GetByUsernameAsync(string username)
        {
            var user = await _dbContext.CmsUsers
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return null;

            return MapToDto(user);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <returns>用户 DTO 列表</returns>
        public async Task<List<UserDto>> GetListAsync(int page, int pageSize, string keyword = null)
        {
            IQueryable<CmsUser> query = _dbContext.CmsUsers
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.Username.Contains(keyword) || u.DisplayName.Contains(keyword));
            }

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return users.Select(MapToDto).ToList();
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="registerDto">注册 DTO</param>
        /// <returns>创建后的用户 DTO</returns>
        public async Task<UserDto> CreateAsync(RegisterDto registerDto)
        {
            var user = new CmsUser
            {
                Username = registerDto.Username,
                PasswordHash = HashPassword(registerDto.Password),
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                IsEnabled = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbContext.CmsUsers.Add(user);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(user.Id);
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="userDto">用户 DTO</param>
        /// <returns>更新后的用户 DTO</returns>
        public async Task<UserDto> UpdateAsync(UserDto userDto)
        {
            var user = await _dbContext.CmsUsers.FindAsync(userDto.Id);
            if (user == null)
                throw new Exception("User not found");

            user.Email = userDto.Email;
            user.DisplayName = userDto.DisplayName;
            user.IsEnabled = userDto.IsEnabled;
            user.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(user.Id);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户 ID</param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var user = await _dbContext.CmsUsers.FindAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 验证用户凭证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>是否验证成功</returns>
        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await _dbContext.CmsUsers.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
            if (user == null)
                return false;

            return VerifyPassword(password, user.PasswordHash);
        }

        /// <summary>
        /// 获取用户角色
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>角色名称列表</returns>
        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            var userRoles = await _dbContext.CmsUserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            return userRoles.Select(ur => ur.Role.Name).ToList();
        }

        /// <summary>
        /// 检查用户权限
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <param name="permissionCode">权限代码</param>
        /// <returns>是否有权限</returns>
        public async Task<bool> CheckPermissionAsync(int userId, string permissionCode)
        {
            // 尝试从缓存获取权限列表
            var permissionsKey = $"UserPermissions_{userId}";
            var permissions = _cacheService.Get<List<string>>(permissionsKey);

            if (permissions == null)
            {
                // 从数据库获取权限列表
                permissions = await _dbContext.CmsUserRoles
                    .Include(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .Where(ur => ur.UserId == userId)
                    .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Code))
                    .Distinct()
                    .ToListAsync();

                // 缓存权限列表
                _cacheService.Set(permissionsKey, permissions, TimeSpan.FromHours(1));
            }

            return permissions.Contains(permissionCode);
        }

        /// <summary>
        /// 将实体映射为 DTO
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <returns>用户 DTO</returns>
        private UserDto MapToDto(CmsUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.DisplayName,
                IsEnabled = user.IsEnabled,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };
        }

        /// <summary>
        /// 对密码进行哈希处理
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <returns>哈希后的密码</returns>
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <param name="hashedPassword">哈希后的密码</param>
        /// <returns>是否匹配</returns>
        private bool VerifyPassword(string password, string hashedPassword)
        {
            // 使用BCrypt验证
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        /// <summary>
        /// 使用SHA256对密码进行哈希处理（兼容旧密码）
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <returns>哈希后的密码</returns>
        private string HashPasswordWithSHA256(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// 生成BCrypt密码
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <returns>BCrypt加密后的密码</returns>
        public string GenerateBCryptPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
