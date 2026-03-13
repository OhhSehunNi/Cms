using Cms.Application.DTOs;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var permission = await _permissionService.GetByIdAsync(id);
            if (permission == null)
            {
                return NotFound();
            }
            return Ok(permission);
        }

        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null)
        {
            var permissions = await _permissionService.GetListAsync(page, pageSize, keyword);
            return Ok(permissions);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PermissionDto permissionDto)
        {
            var permission = await _permissionService.CreateAsync(permissionDto);
            return CreatedAtAction(nameof(GetById), new { id = permission.Id }, permission);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PermissionDto permissionDto)
        {
            if (id != permissionDto.Id)
            {
                return BadRequest();
            }
            var updatedPermission = await _permissionService.UpdateAsync(permissionDto);
            return Ok(updatedPermission);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _permissionService.DeleteAsync(id);
            return NoContent();
        }
    }
}