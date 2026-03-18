using Cms.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Cms.WebApi.Filters
{
    /// <summary>
    /// JWT认证过滤器
    /// </summary>
    public class JwtAuthenticationFilter : IAuthorizationFilter
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="authService">认证服务</param>
        public JwtAuthenticationFilter(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// 授权过滤器
        /// </summary>
        /// <param name="context">授权上下文</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // 跳过匿名访问 - 使用 Endpoint Metadata 更可靠（支持 Endpoint Routing）
            var allowAnonymous = context.HttpContext.GetEndpoint()?.Metadata?.OfType<AllowAnonymousAttribute>().Any() ?? false;
            if (allowAnonymous)
            {
                return;
            }

            // 打印授权头
            var authHeader = context.HttpContext.Request.Headers.Authorization.ToString();
            //Console.WriteLine($"Authorization header: {authHeader}");

            // 检查授权头格式并尝试去除额外包裹
            if (!string.IsNullOrEmpty(authHeader))
            {
                var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? authHeader.Substring(7).Trim()
                    : authHeader.Trim();

                if (token.StartsWith("{") && token.EndsWith("}"))
                {
                    token = token.Substring(1, token.Length - 2).Trim();
                    context.HttpContext.Request.Headers.Authorization = "Bearer " + token;
                   // Console.WriteLine($"Updated authorization header: Bearer {token}");
                }
            }

            // 检查用户是否已认证
            var isAuthenticated = context.HttpContext.User.Identity?.IsAuthenticated ?? false;
            //Console.WriteLine($"IsAuthenticated: {isAuthenticated}");

            //if (context.HttpContext.User.Identity != null)
            //{
            //    Console.WriteLine($"Identity type: {context.HttpContext.User.Identity.GetType().Name}");
            //}

            //// 打印所有claims
            //var claims = context.HttpContext.User.Claims.ToList();
            //Console.WriteLine($"Number of claims: {claims.Count}");
            //foreach (var claim in claims)
            //{
            //    Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
            //}

            if (!isAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // 获取用户ID
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) ??
                             context.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // 验证用户ID格式
            if (!int.TryParse(userIdClaim.Value, out _))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }

    /// <summary>
    /// 权限过滤器属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class PermissionAuthorizeAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="permission">权限代码</param>
        public PermissionAuthorizeAttribute(string permission)
            : base(typeof(PermissionAuthorizationFilter))
        {
            Arguments = new object[] { permission };
        }
    }

    /// <summary>
    /// 权限过滤器
    /// </summary>
    public class PermissionAuthorizationFilter : IAuthorizationFilter
    {
        private readonly IUserService _userService;
        private readonly string _permission;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userService">用户服务</param>
        /// <param name="permission">权限代码</param>
        public PermissionAuthorizationFilter(IUserService userService, string permission)
        {
            _userService = userService;
            _permission = permission;
        }

        /// <summary>
        /// 授权过滤器
        /// </summary>
        /// <param name="context">授权上下文</param>
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            // 检查用户是否已认证
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // 获取用户ID
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) ?? 
                             context.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userId = int.Parse(userIdClaim.Value);

            // 检查权限
            var hasPermission = await _userService.CheckPermissionAsync(userId, _permission);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}