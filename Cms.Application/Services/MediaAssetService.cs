using Cms.Application.DTOs;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Cms.Application.Services
{
    public class MediaAssetService : IMediaAssetService
    {
        private readonly CmsDbContext _dbContext;
        private readonly string _uploadPath;

        public MediaAssetService(CmsDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _uploadPath = configuration["UploadPath"] ?? "wwwroot/uploads";
            
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<MediaAssetDto> GetByIdAsync(int id)
        {
            var asset = await _dbContext.CmsMediaAssets.FindAsync(id);
            if (asset == null)
                return null;

            return MapToDto(asset);
        }

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

        public async Task<List<string>> GetGroupsAsync()
        {
            var groups = await _dbContext.CmsMediaAssets
                .Where(a => !string.IsNullOrEmpty(a.Group))
                .Select(a => a.Group)
                .Distinct()
                .ToListAsync();

            return groups;
        }

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