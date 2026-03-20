using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 用户管理控制器
    /// 提供用户的CRUD操作以及权限检查等功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// 用户服务接口
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userService">用户服务实例</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>用户列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null)
        {
            try
            {
                var users = await _userService.GetListAsync(page, pageSize, keyword);
                var total = await _userService.GetCountAsync(keyword);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        list = users,
                        total = total,
                        page = page,
                        pageSize = pageSize
                    },
                    message = "获取用户列表成功"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "用户不存在" });
                }
                return Ok(new { success = true, data = user, message = "获取用户信息成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="registerDto">注册信息</param>
        /// <returns>创建的用户信息</returns>
        [HttpPost]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> Create([FromBody] RegisterDto registerDto)
        {
            try
            {
                var user = await _userService.CreateAsync(registerDto);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, new { success = true, data = user, message = "创建用户成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="userDto">用户信息</param>
        /// <returns>更新后的用户信息</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto userDto)
        {
            try
            {
                if (id != userDto.Id)
                {
                    return BadRequest(new { success = false, message = "用户ID不匹配" });
                }
                var updatedUser = await _userService.UpdateAsync(userDto);
                return Ok(new { success = true, data = updatedUser, message = "更新用户信息成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                return Ok(new { success = true, message = "删除用户成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 获取用户角色
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户角色列表</returns>
        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetUserRoles(int id)
        {
            try
            {
                var roles = await _userService.GetUserRolesAsync(id);
                return Ok(new { success = true, data = roles, message = "获取用户角色成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 分配用户角色
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="request">角色分配请求</param>
        /// <returns>分配结果</returns>
        [HttpPost("{id}/roles")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> AssignRoles(int id, [FromBody] AssignRolesRequest request)
        {
            try
            {
                await _userService.AssignRolesAsync(id, request.RoleIds);
                return Ok(new { success = true, message = "分配用户角色成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 检查用户权限
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="request">权限检查请求</param>
        /// <returns>权限检查结果</returns>
        [HttpPost("{id}/check-permission")]
        public async Task<IActionResult> CheckPermission(int id, [FromBody] CheckPermissionRequest request)
        {
            try
            {
                var hasPermission = await _userService.CheckPermissionAsync(id, request.PermissionCode);
                return Ok(new { success = true, data = new { hasPermission }, message = "权限检查完成" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="request">密码重置请求</param>
        /// <returns>重置结果</returns>
        [HttpPost("{id}/reset-password")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _userService.ResetPasswordAsync(id, request.NewPassword);
                return Ok(new { success = true, message = "重置用户密码成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 权限检查请求类
        /// </summary>
        public class CheckPermissionRequest
        {
            /// <summary>
            /// 权限代码
            /// </summary>
            public string PermissionCode { get; set; }
        }

        /// <summary>
        /// 角色分配请求类
        /// </summary>
        public class AssignRolesRequest
        {
            /// <summary>
            /// 角色ID列表
            /// </summary>
            public List<int> RoleIds { get; set; }
        }

        /// <summary>
        /// 密码重置请求类
        /// </summary>
        public class ResetPasswordRequest
        {
            /// <summary>
            /// 新密码
            /// </summary>
            public string NewPassword { get; set; }
        }
    }
}