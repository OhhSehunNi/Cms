using Cms.Application.DTOs;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cms.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly CmsDbContext _dbContext;

        public RoleService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RoleDto> GetByIdAsync(int id)
        {
            var role = await _dbContext.CmsRoles
                .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return null;

            return MapToDto(role);
        }

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

        public async Task DeleteAsync(int id)
        {
            var role = await _dbContext.CmsRoles.FindAsync(id);
            if (role != null)
            {
                role.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }

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

        public async Task UpdateRolePermissionsAsync(int roleId, List<int> permissionIds)
        {
            // 先删除现有的权限关联
            var existingPermissions = _dbContext.CmsRolePermissions.Where(rp => rp.RoleId == roleId);
            _dbContext.CmsRolePermissions.RemoveRange(existingPermissions);

            // 添加新的权限关联
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