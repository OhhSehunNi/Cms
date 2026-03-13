using Cms.Application.DTOs;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cms.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly CmsDbContext _dbContext;

        public PermissionService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PermissionDto> GetByIdAsync(int id)
        {
            var permission = await _dbContext.CmsPermissions.FindAsync(id);
            if (permission == null)
                return null;

            return MapToDto(permission);
        }

        public async Task<List<PermissionDto>> GetListAsync(int page, int pageSize, string? keyword = null)
        {
            IQueryable<CmsPermission> query = _dbContext.CmsPermissions;

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword) || p.Code.Contains(keyword) || p.Description.Contains(keyword));
            }

            var permissions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return permissions.Select(MapToDto).ToList();
        }

        public async Task<PermissionDto> CreateAsync(PermissionDto permissionDto)
        {
            var permission = new CmsPermission
            {
                Code = permissionDto.Code,
                Name = permissionDto.Name,
                Description = permissionDto.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbContext.CmsPermissions.Add(permission);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(permission.Id);
        }

        public async Task<PermissionDto> UpdateAsync(PermissionDto permissionDto)
        {
            var permission = await _dbContext.CmsPermissions.FindAsync(permissionDto.Id);
            if (permission == null)
                throw new Exception("Permission not found");

            permission.Code = permissionDto.Code;
            permission.Name = permissionDto.Name;
            permission.Description = permissionDto.Description;
            permission.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(permission.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var permission = await _dbContext.CmsPermissions.FindAsync(id);
            if (permission != null)
            {
                permission.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        private PermissionDto MapToDto(CmsPermission permission)
        {
            return new PermissionDto
            {
                Id = permission.Id,
                Code = permission.Code,
                Name = permission.Name,
                Description = permission.Description
            };
        }
    }
}