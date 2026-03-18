using Cms.Application.Services.Dtos;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Application.Services
{
    /// <summary>
    /// 认证服务，用于处理JWT Token相关操作
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly CmsDbContext _dbContext;
        private readonly ICacheService _cacheService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="cacheService">缓存服务</param>
        /// <param name="configuration">配置</param>
        public AuthService(CmsDbContext dbContext, ICacheService cacheService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
            _configuration = configuration;
        }

        /// <summary>
        /// 生成AccessToken和RefreshToken
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <param name="websiteId">网站ID</param>
        /// <returns>Token响应</returns>
        public async Task<TokenResponseDto> GenerateTokens(CmsUser user, int websiteId)
        {
            var roles = await _dbContext.CmsUserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.Role.Name)
                .ToListAsync();

            var permissions = await _dbContext.CmsUserRoles
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Where(ur => ur.UserId == user.Id)
                .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Code))
                .Distinct()
                .ToListAsync();

            var channelIds = await _dbContext.CmsUserRoles
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RoleChannels)
                .Where(ur => ur.UserId == user.Id)
                .SelectMany(ur => ur.Role.RoleChannels.Select(rc => rc.ChannelId))
                .Distinct()
                .ToListAsync();

            var accessToken = GenerateAccessToken(user, websiteId, roles, permissions, channelIds);
            var refreshToken = GenerateRefreshToken();

            // 存储RefreshToken到缓存
            var refreshTokenKey = $"RefreshToken_{user.Id}";
            _cacheService.Set(refreshTokenKey, refreshToken, TimeSpan.FromDays(7));

            // 缓存用户权限
            var permissionsKey = $"UserPermissions_{user.Id}";
            _cacheService.Set(permissionsKey, permissions, TimeSpan.FromHours(1));

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 30 * 60, // 30分钟
                UserId = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Roles = roles,
                Permissions = permissions
            };
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="refreshToken">刷新Token</param>
        /// <param name="userId">用户ID</param>
        /// <returns>新的Token响应</returns>
        public async Task<TokenResponseDto> RefreshToken(string refreshToken, int userId)
        {
            // 验证RefreshToken
            var refreshTokenKey = $"RefreshToken_{userId}";
            var storedRefreshToken = _cacheService.Get<string>(refreshTokenKey);

            if (storedRefreshToken != refreshToken)
            {
                throw new Exception("Invalid refresh token");
            }

            // 获取用户信息
            var user = await _dbContext.CmsUsers.FindAsync(userId);
            if (user == null || !user.IsEnabled || user.IsDeleted)
            {
                throw new Exception("User not found or disabled");
            }

            // 生成新的Tokens
            return await GenerateTokens(user, 1); // 暂时使用默认网站ID
        }

        /// <summary>
        /// 吊销Token
        /// </summary>
        /// <param name="userId">用户ID</param>
        public void RevokeToken(int userId)
        {
            var refreshTokenKey = $"RefreshToken_{userId}";
            _cacheService.Remove(refreshTokenKey);

            var permissionsKey = $"UserPermissions_{userId}";
            _cacheService.Remove(permissionsKey);
        }

        /// <summary>
        /// 检查Token是否已吊销
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>是否已吊销</returns>
        public bool IsTokenRevoked(int userId)
        {
            var refreshTokenKey = $"RefreshToken_{userId}";
            return !_cacheService.Exists(refreshTokenKey);
        }

        /// <summary>
        /// 验证用户凭证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>用户实体</returns>
        public async Task<CmsUser> ValidateUser(string username, string password)
        {
            var user = await _dbContext.CmsUsers.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
            if (user == null)
            {
                throw new Exception("用户名或密码错误");
            }

            if (!user.IsEnabled)
            {
                throw new Exception("账号已禁用");
            }

            // 检查登录失败次数
            var failedAttemptsKey = $"LoginFailed_{username}";
            var failedAttempts = _cacheService.Get<int>(failedAttemptsKey);

            if (failedAttempts >= 5)
            {
                throw new Exception("账号已锁定");
            }

            // 验证密码 - 使用BCrypt加密方式
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                // 增加失败次数
                _cacheService.Increment(failedAttemptsKey, TimeSpan.FromMinutes(15));
                throw new Exception("用户名或密码错误");
            }

            // 重置失败次数
            _cacheService.Remove(failedAttemptsKey);

            return user;
        }

        /// <summary>
        /// 生成AccessToken
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <param name="websiteId">网站ID</param>
        /// <param name="roles">角色列表</param>
        /// <param name="permissions">权限列表</param>
        /// <param name="channelIds">栏目ID列表</param>
        /// <returns>AccessToken</returns>
        private string GenerateAccessToken(CmsUser user, int websiteId, List<string> roles, List<string> permissions, List<int> channelIds)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim("websiteId", websiteId.ToString()),
                new Claim("displayName", user.DisplayName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            foreach (var channelId in channelIds)
            {
                claims.Add(new Claim("channelId", channelId.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 生成RefreshToken
        /// </summary>
        /// <returns>RefreshToken</returns>
        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
