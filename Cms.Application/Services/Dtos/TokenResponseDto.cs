using System.Collections.Generic;

namespace Cms.Application.Services.Dtos
{
    /// <summary>
    /// Token响应数据传输对象
    /// </summary>
    public class TokenResponseDto
    {
        /// <summary>
        /// AccessToken
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// RefreshToken
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// 过期时间（秒）
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 角色列表
        /// </summary>
        public List<string> Roles { get; set; }

        /// <summary>
        /// 权限列表
        /// </summary>
        public List<string> Permissions { get; set; }
    }

    /// <summary>
    /// 刷新Token请求数据传输对象
    /// </summary>
    public class RefreshTokenRequestDto
    {
        /// <summary>
        /// RefreshToken
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
    }

    /// <summary>
    /// 登录请求数据传输对象
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string Captcha { get; set; }

        /// <summary>
        /// 网站ID
        /// </summary>
        public int WebsiteId { get; set; }
    }

    /// <summary>
    /// 权限检查请求数据传输对象
    /// </summary>
    public class CheckPermissionRequestDto
    {
        /// <summary>
        /// 权限代码
        /// </summary>
        public string PermissionCode { get; set; }
    }
}