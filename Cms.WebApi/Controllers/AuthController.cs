using Cms.Application.DTOs;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var isValid = await _userService.ValidateCredentialsAsync(loginDto.Username, loginDto.Password);
            if (!isValid)
            {
                return Unauthorized();
            }
            var user = await _userService.GetByUsernameAsync(loginDto.Username);
            return Ok(user);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // 实现登出逻辑
            return Ok();
        }
    }

    public class LoginRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}