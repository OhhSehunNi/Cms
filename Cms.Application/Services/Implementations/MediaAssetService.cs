using Cms.Application.Services.Dtos;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Cms.Application.Services
{
    /// <summary>
    /// 媒体资源服务实现类，用于媒体资源相关的业务逻辑
    /// </summary>
    public class MediaAssetService : IMediaAssetService
    {
        private readonly CmsDbContext _dbContext;
        private readonly string _uploadPath;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="configuration">配置</param>
        public MediaAssetService(CmsDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _uploadPath = configuration["UploadPath"] ?? "wwwroot/uploads";
            
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        /// <summary>
        /// 根据 ID 获取媒体资源
        /// </summary>
        /// <param name="id">资源 ID</param>
        /// <returns>媒体资源 DTO</returns>
        public async Task<MediaAssetDto> GetByIdAsync(int id)
        {
            var asset = await _dbContext.CmsMediaAssets.FindAsync(id);
            if (asset == null)
                return null;

            return MapToDto(asset);
        }

        /// <summary>
        /// 获取媒体资源列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <param name="group">分组</param>
        /// <returns>媒体资源 DTO 列表</returns>
        public async Task<List<MediaAssetDto>> GetListAsync(int page, int pageSize, string keyword = null, string group = null)
        {
            var query = _dbContext.CmsMediaAssets.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.Name.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(group))
            {
                query = query.Where(a => a.Group == group);
            }

            var assets = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return assets.Select(MapToDto).ToList();
        }

        /// <summary>
        /// 上传媒体资源
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="fileData">文件数据</param>
        /// <param name="group">分组</param>
        /// <returns>上传后的媒体资源 DTO</returns>
        public async Task<MediaAssetDto> UploadAsync(string fileName, string contentType, long fileSize, byte[] fileData, string group = null)
        {
            var extension = Path.GetExtension(fileName);
            var newFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadPath, newFileName);

            await File.WriteAllBytesAsync(filePath, fileData);

            var asset = new CmsMediaAsset
            {
                Name = fileName,
                Type = contentType,
                Path = filePath,
                Url = $"/uploads/{newFileName}",
                Size = fileSize,
                Group = group,
                Extension = extension,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbContext.CmsMediaAssets.Add(asset);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(asset.Id);
        }

        /// <summary>
        /// 删除媒体资源
        /// </summary>
        /// <param name="id">资源 ID</param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var asset = await _dbContext.CmsMediaAssets.FindAsync(id);
            if (asset != null)
            {
                if (File.Exists(asset.Path))
                {
                    File.Delete(asset.Path);
                }

                asset.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 获取分组列表
        /// </summary>
        /// <returns>分组名称列表</returns>
        public async Task<List<string>> GetGroupsAsync()
        {
            var groups = await _dbContext.CmsMediaAssets
                .Where(a => !string.IsNullOrEmpty(a.Group))
                .Select(a => a.Group)
                .Distinct()
                .ToListAsync();

            return groups;
        }

        /// <summary>
        /// 将实体映射为 DTO
        /// </summary>
        /// <param name="asset">媒体资源实体</param>
        /// <returns>媒体资源 DTO</returns>
        private MediaAssetDto MapToDto(CmsMediaAsset asset)
        {
            return new MediaAssetDto
            {
                Id = asset.Id,
                Name = asset.Name,
                Type = asset.Type,
                Path = asset.Path,
                Url = asset.Url,
                Size = asset.Size,
                Group = asset.Group,
                Extension = asset.Extension,
                SizeFormatted = FormatFileSize(asset.Size)
            };
        }

        /// <summary>
        /// 格式化文件大小
        /// </summary>
        /// <param name="bytes">字节数</param>
        /// <returns>格式化后的文件大小</returns>
        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }
    }
}