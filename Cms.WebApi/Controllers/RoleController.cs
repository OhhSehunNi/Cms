using Cms.Application.DTOs;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }

        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null)
        {
            var roles = await _roleService.GetListAsync(page, pageSize, keyword);
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleDto roleDto)
        {
            var role = await _roleService.CreateAsync(roleDto);
            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoleDto roleDto)
        {
            if (id != roleDto.Id)
            {
                return BadRequest();
            }
            var updatedRole = await _roleService.UpdateAsync(roleDto);
            return Ok(updatedRole);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _roleService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetRolePermissions(int id)
        {
            var permissions = await _roleService.GetRolePermissionsAsync(id);
            return Ok(permissions);
        }

        [HttpPost("{id}/permissions")]
        public async Task<IActionResult> UpdateRolePermissions(int id, [FromBody] UpdateRolePermissionsRequest request)
        {
            await _roleService.UpdateRolePermissionsAsync(id, request.PermissionIds);
            return Ok();
        }

        public class UpdateRolePermissionsRequest
        {
            public List<int> PermissionIds { get; set; }
        }
    }
}