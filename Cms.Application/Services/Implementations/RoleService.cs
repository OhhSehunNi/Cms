using Cms.Application.Services.Dtos;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cms.Application.Services
{
    /// <summary>
    /// 角色服务实现类，用于角色相关的业务逻辑
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly CmsDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public RoleService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 根据 ID 获取角色
        /// </summary>
        /// <param name="id">角色 ID</param>
        /// <returns>角色 DTO</returns>
        public async Task<RoleDto> GetByIdAsync(int id)
        {
            var role = await _dbContext.CmsRoles
                .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return null;

            return MapToDto(role);
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <returns>角色 DTO 列表</returns>
        public async Task<List<RoleDto>> GetListAsync(int page, int pageSize, string? keyword = null)
        {
            IQueryable<CmsRole> query = _dbContext.CmsRoles
                .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(r => r.Name.Contains(keyword) || r.Description.Contains(keyword));
            }

            var roles = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return roles.Select(MapToDto).ToList();
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="roleDto">角色 DTO</param>
        /// <returns>创建后的角色 DTO</returns>
        public async Task<RoleDto> CreateAsync(RoleDto roleDto)
        {
            var role = new CmsRole
            {
                Name = roleDto.Name,
                Description = roleDto.Description,
                IsEnabled = roleDto.IsEnabled,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbContext.CmsRoles.Add(role);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(role.Id);
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="roleDto">角色 DTO</param>
        /// <returns>更新后的角色 DTO</returns>
        public async Task<RoleDto> UpdateAsync(RoleDto roleDto)
        {
            var role = await _dbContext.CmsRoles.FindAsync(roleDto.Id);
            if (role == null)
                throw new Exception("Role not found");

            role.Name = roleDto.Name;
            role.Description = roleDto.Description;
            role.IsEnabled = roleDto.IsEnabled;
            role.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(role.Id);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色 ID</param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var role = await _dbContext.CmsRoles.FindAsync(id);
            if (role != null)
            {
                role.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="roleId">角色 ID</param>
        /// <returns>权限 DTO 列表</returns>
        public async Task<List<PermissionDto>> GetRolePermissionsAsync(int roleId)
        {
            var rolePermissions = await _dbContext.CmsRolePermissions
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            return rolePermissions.Select(rp => new PermissionDto
            {
                Id = rp.Permission.Id,
                Code = rp.Permission.Code,
                Name = rp.Permission.Name,
                Description = rp.Permission.Description
            }).ToList();
        }

        /// <summary>
        /// 更新角色权限
        /// </summary>
        /// <param name="roleId">角色 ID</param>
        /// <param name="permissionIds">权限 ID 列表</param>
        /// <returns></returns>
        public async Task UpdateRolePermissionsAsync(int roleId, List<int> permissionIds)
        {
            var existingPermissions = _dbContext.CmsRolePermissions.Where(rp => rp.RoleId == roleId);
            _dbContext.CmsRolePermissions.RemoveRange(existingPermissions);

            foreach (var permissionId in permissionIds)
            {
                var rolePermission = new CmsRolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                };
                _dbContext.CmsRolePermissions.Add(rolePermission);
            }

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 将实体映射为 DTO
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <returns>角色 DTO</returns>
        private RoleDto MapToDto(CmsRole role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsEnabled = role.IsEnabled,
                Permissions = role.RolePermissions.Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Code = rp.Permission.Code,
                    Name = rp.Permission.Name,
                    Description = rp.Permission.Description
                }).ToList()
            };
        }
    }
}
