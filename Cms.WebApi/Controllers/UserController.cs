using Cms.Application.DTOs;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null)
        {
            var users = await _userService.GetListAsync(page, pageSize, keyword);
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegisterDto registerDto)
        {
            var user = await _userService.CreateAsync(registerDto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto userDto)
        {
            if (id != userDto.Id)
            {
                return BadRequest();
            }
            var updatedUser = await _userService.UpdateAsync(userDto);
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetUserRoles(int id)
        {
            var roles = await _userService.GetUserRolesAsync(id);
            return Ok(roles);
        }

        [HttpPost("{id}/check-permission")]
        public async Task<IActionResult> CheckPermission(int id, [FromBody] CheckPermissionRequest request)
        {
            var hasPermission = await _userService.CheckPermissionAsync(id, request.PermissionCode);
            return Ok(new { hasPermission });
        }

        public class CheckPermissionRequest
        {
            public string PermissionCode { get; set; }
        }
    }
}