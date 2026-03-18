using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 角色管理控制器
    /// 提供角色的CRUD操作以及权限管理功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        /// <summary>
        /// 角色服务接口
        /// </summary>
        private readonly IRoleService _roleService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="roleService">角色服务实例</param>
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// 根据ID获取角色信息
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>角色信息</returns>
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

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>角色列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null)
        {
            var roles = await _roleService.GetListAsync(page, pageSize, keyword);
            return Ok(roles);
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="roleDto">角色信息</param>
        /// <returns>创建的角色信息</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleDto roleDto)
        {
            var role = await _roleService.CreateAsync(roleDto);
            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <param name="roleDto">角色信息</param>
        /// <returns>更新后的角色信息</returns>
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

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _roleService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>角色权限列表</returns>
        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetRolePermissions(int id)
        {
            var permissions = await _roleService.GetRolePermissionsAsync(id);
            return Ok(permissions);
        }

        /// <summary>
        /// 更新角色权限
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <param name="request">权限更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPost("{id}/permissions")]
        public async Task<IActionResult> UpdateRolePermissions(int id, [FromBody] UpdateRolePermissionsRequest request)
        {
            await _roleService.UpdateRolePermissionsAsync(id, request.PermissionIds);
            return Ok();
        }

        /// <summary>
        /// 更新角色权限请求类
        /// </summary>
        public class UpdateRolePermissionsRequest
        {
            /// <summary>
            /// 权限ID列表
            /// </summary>
            public List<int> PermissionIds { get; set; }
        }
    }
}