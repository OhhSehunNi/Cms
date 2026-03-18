using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 认证控制器
    /// 提供登录、登出、Token管理和权限检查功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// 认证服务接口
        /// </summary>
        private readonly IAuthService _authService;

        /// <summary>
        /// 用户服务接口
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// 登录日志服务接口
        /// </summary>
        private readonly ILoginLogService _loginLogService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="authService">认证服务实例</param>
        /// <param name="userService">用户服务实例</param>
        /// <param name="loginLogService">登录日志服务实例</param>
        public AuthController(IAuthService authService, IUserService userService, ILoginLogService loginLogService)
        {
            _authService = authService;
            _userService = userService;
            _loginLogService = loginLogService;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginDto">登录信息</param>
        /// <returns>Token响应</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

            try
            {
                // 验证用户
                var user = await _authService.ValidateUser(loginDto.Username, loginDto.Password);
                
                // 生成Token
                var tokenResponse = await _authService.GenerateTokens(user, loginDto.WebsiteId);
                
                // 记录登录成功日志
                await _loginLogService.LogLoginAsync(user.Id, user.Username, ip, userAgent, true, "登录成功");
                
                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                // 记录登录失败日志
                await _loginLogService.LogLoginAsync(null, loginDto.Username, ip, userAgent, false, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="refreshTokenDto">刷新Token请求</param>
        /// <returns>新的Token响应</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenDto)
        {
            try
            {
                var tokenResponse = await _authService.RefreshToken(refreshTokenDto.RefreshToken, refreshTokenDto.UserId);
                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <returns>登出结果</returns>
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _authService.RevokeToken(userId);
                return Ok(new { message = "登出成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns>用户信息</returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "用户不存在" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 检查用户权限
        /// </summary>
        /// <param name="requestDto">权限检查请求</param>
        /// <returns>权限检查结果</returns>
        [HttpPost("check-permission")]
        [Authorize]
        public async Task<IActionResult> CheckPermission([FromBody] CheckPermissionRequestDto requestDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var hasPermission = await _userService.CheckPermissionAsync(userId, requestDto.PermissionCode);
                return Ok(new { hasPermission });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 生成BCrypt密码
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <returns>BCrypt加密后的密码</returns>
        [HttpPost("generate-password")]
        public IActionResult GeneratePassword([FromBody] string password)
        {
            try
            {
                var hashedPassword = _userService.GenerateBCryptPassword(password);
                return Ok(new { password, hashedPassword });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 测试登录功能
        /// </summary>
        /// <returns>登录测试结果</returns>
        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult TestLogin()
        {
            return Ok(new { message = "登录接口测试成功" });
        }
    }
}