using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 权限管理控制器
    /// 提供权限的CRUD操作
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        /// <summary>
        /// 权限服务接口
        /// </summary>
        private readonly IPermissionService _permissionService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="permissionService">权限服务实例</param>
        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// 根据ID获取权限信息
        /// </summary>
        /// <param name="id">权限ID</param>
        /// <returns>权限信息</returns>
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

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>权限列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null)
        {
            var permissions = await _permissionService.GetListAsync(page, pageSize, keyword);
            return Ok(permissions);
        }

        /// <summary>
        /// 创建权限
        /// </summary>
        /// <param name="permissionDto">权限信息</param>
        /// <returns>创建的权限信息</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PermissionDto permissionDto)
        {
            var permission = await _permissionService.CreateAsync(permissionDto);
            return CreatedAtAction(nameof(GetById), new { id = permission.Id }, permission);
        }

        /// <summary>
        /// 更新权限信息
        /// </summary>
        /// <param name="id">权限ID</param>
        /// <param name="permissionDto">权限信息</param>
        /// <returns>更新后的权限信息</returns>
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

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id">权限ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _permissionService.DeleteAsync(id);
            return NoContent();
        }
    }
}