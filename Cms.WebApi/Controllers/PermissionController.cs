using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 权限管理控制器
    /// 提供权限的CRUD操作
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        /// 获取权限列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>权限列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null)
        {
            try
            {
                var permissions = await _permissionService.GetListAsync(page, pageSize, keyword);
                var total = await _permissionService.GetCountAsync(keyword);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        list = permissions,
                        total = total,
                        page = page,
                        pageSize = pageSize
                    },
                    message = "获取权限列表成功"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取权限信息
        /// </summary>
        /// <param name="id">权限ID</param>
        /// <returns>权限信息</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var permission = await _permissionService.GetByIdAsync(id);
                if (permission == null)
                {
                    return NotFound(new { success = false, message = "权限不存在" });
                }
                return Ok(new { success = true, data = permission, message = "获取权限信息成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 创建权限
        /// </summary>
        /// <param name="permissionDto">权限信息</param>
        /// <returns>创建的权限信息</returns>
        [HttpPost]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> Create([FromBody] PermissionDto permissionDto)
        {
            try
            {
                var permission = await _permissionService.CreateAsync(permissionDto);
                return CreatedAtAction(nameof(GetById), new { id = permission.Id }, new { success = true, data = permission, message = "创建权限成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 更新权限信息
        /// </summary>
        /// <param name="id">权限ID</param>
        /// <param name="permissionDto">权限信息</param>
        /// <returns>更新后的权限信息</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> Update(int id, [FromBody] PermissionDto permissionDto)
        {
            try
            {
                if (id != permissionDto.Id)
                {
                    return BadRequest(new { success = false, message = "权限ID不匹配" });
                }
                var updatedPermission = await _permissionService.UpdateAsync(permissionDto);
                return Ok(new { success = true, data = updatedPermission, message = "更新权限信息成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id">权限ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _permissionService.DeleteAsync(id);
                return Ok(new { success = true, message = "删除权限成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 获取权限分类列表
        /// </summary>
        /// <returns>权限分类列表</returns>
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _permissionService.GetCategoriesAsync();
                return Ok(new { success = true, data = categories, message = "获取权限分类列表成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}