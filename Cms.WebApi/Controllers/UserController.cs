using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 用户管理控制器
    /// 提供用户的CRUD操作以及权限检查等功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
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
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息</returns>
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
            var users = await _userService.GetListAsync(page, pageSize, keyword);
            return Ok(users);
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="registerDto">注册信息</param>
        /// <returns>创建的用户信息</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegisterDto registerDto)
        {
            var user = await _userService.CreateAsync(registerDto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="userDto">用户信息</param>
        /// <returns>更新后的用户信息</returns>
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

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// 获取用户角色
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户角色列表</returns>
        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetUserRoles(int id)
        {
            var roles = await _userService.GetUserRolesAsync(id);
            return Ok(roles);
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
            var hasPermission = await _userService.CheckPermissionAsync(id, request.PermissionCode);
            return Ok(new { hasPermission });
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
    }
}