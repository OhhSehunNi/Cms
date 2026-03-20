using Cms.Application.Services.Dtos;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cms.Application.Services
{
    /// <summary>
    /// 权限服务实现类，用于权限相关的业务逻辑
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly CmsDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public PermissionService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 根据 ID 获取权限
        /// </summary>
        /// <param name="id">权限 ID</param>
        /// <returns>权限 DTO</returns>
        public async Task<PermissionDto> GetByIdAsync(int id)
        {
            var permission = await _dbContext.CmsPermissions.FindAsync(id);
            if (permission == null)
                return null;

            return MapToDto(permission);
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <returns>权限 DTO 列表</returns>
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

        /// <summary>
        /// 创建权限
        /// </summary>
        /// <param name="permissionDto">权限 DTO</param>
        /// <returns>创建后的权限 DTO</returns>
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

        /// <summary>
        /// 更新权限
        /// </summary>
        /// <param name="permissionDto">权限 DTO</param>
        /// <returns>更新后的权限 DTO</returns>
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

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id">权限 ID</param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var permission = await _dbContext.CmsPermissions.FindAsync(id);
            if (permission != null)
            {
                permission.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 获取权限总数
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns>权限总数</returns>
        public async Task<int> GetCountAsync(string? keyword = null)
        {
            IQueryable<CmsPermission> query = _dbContext.CmsPermissions;

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword) || p.Code.Contains(keyword) || p.Description.Contains(keyword));
            }

            return await query.CountAsync();
        }

        /// <summary>
        /// 获取权限分类列表
        /// </summary>
        /// <returns>权限分类列表</returns>
        public async Task<List<string>> GetCategoriesAsync()
        {
            var permissions = await _dbContext.CmsPermissions
                .Select(p => p.Code)
                .ToListAsync();

            var categories = permissions
                .Select(p => p.Split('.')[0])
                .Distinct()
                .ToList();

            return categories;
        }

        /// <summary>
        /// 将实体映射为 DTO
        /// </summary>
        /// <param name="permission">权限实体</param>
        /// <returns>权限 DTO</returns>
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
