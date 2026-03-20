using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 角色管理控制器
    /// 提供角色的CRUD操作以及权限管理功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        /// 获取角色列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>角色列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null)
        {
            try
            {
                var roles = await _roleService.GetListAsync(page, pageSize, keyword);
                var total = await _roleService.GetCountAsync(keyword);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        list = roles,
                        total = total,
                        page = page,
                        pageSize = pageSize
                    },
                    message = "获取角色列表成功"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取角色信息
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>角色信息</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var role = await _roleService.GetByIdAsync(id);
                if (role == null)
                {
                    return NotFound(new { success = false, message = "角色不存在" });
                }
                return Ok(new { success = true, data = role, message = "获取角色信息成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="roleDto">角色信息</param>
        /// <returns>创建的角色信息</returns>
        [HttpPost]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> Create([FromBody] RoleDto roleDto)
        {
            try
            {
                var role = await _roleService.CreateAsync(roleDto);
                return CreatedAtAction(nameof(GetById), new { id = role.Id }, new { success = true, data = role, message = "创建角色成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <param name="roleDto">角色信息</param>
        /// <returns>更新后的角色信息</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> Update(int id, [FromBody] RoleDto roleDto)
        {
            try
            {
                if (id != roleDto.Id)
                {
                    return BadRequest(new { success = false, message = "角色ID不匹配" });
                }
                var updatedRole = await _roleService.UpdateAsync(roleDto);
                return Ok(new { success = true, data = updatedRole, message = "更新角色信息成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _roleService.DeleteAsync(id);
                return Ok(new { success = true, message = "删除角色成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>角色权限列表</returns>
        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetRolePermissions(int id)
        {
            try
            {
                var permissions = await _roleService.GetRolePermissionsAsync(id);
                return Ok(new { success = true, data = permissions, message = "获取角色权限成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 更新角色权限
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <param name="request">权限更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPost("{id}/permissions")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> UpdateRolePermissions(int id, [FromBody] UpdateRolePermissionsRequest request)
        {
            try
            {
                await _roleService.UpdateRolePermissionsAsync(id, request.PermissionIds);
                return Ok(new { success = true, message = "更新角色权限成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 获取角色可管理的栏目
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>栏目ID列表</returns>
        [HttpGet("{id}/channels")]
        public async Task<IActionResult> GetRoleChannels(int id)
        {
            try
            {
                var channels = await _roleService.GetRoleChannelsAsync(id);
                return Ok(new { success = true, data = channels, message = "获取角色栏目权限成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 更新角色栏目权限
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <param name="request">栏目权限更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPost("{id}/channels")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> UpdateRoleChannels(int id, [FromBody] UpdateRoleChannelsRequest request)
        {
            try
            {
                await _roleService.UpdateRoleChannelsAsync(id, request.ChannelIds);
                return Ok(new { success = true, message = "更新角色栏目权限成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
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

        /// <summary>
        /// 更新角色栏目权限请求类
        /// </summary>
        public class UpdateRoleChannelsRequest
        {
            /// <summary>
            /// 栏目ID列表
            /// </summary>
            public List<int> ChannelIds { get; set; }
        }
    }
}