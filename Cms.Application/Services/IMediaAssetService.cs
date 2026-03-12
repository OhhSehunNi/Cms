using Cms.Application.DTOs;

namespace Cms.Application.Services
{
    public interface IMediaAssetService
    {
        Task<MediaAssetDto> GetByIdAsync(int id);
        Task<List<MediaAssetDto>> GetListAsync(int page, int pageSize, string? keyword = null, string? group = null);
        Task<MediaAssetDto> UploadAsync(string fileName, string contentType, long fileSize, byte[] fileData, string? group = null);
        Task DeleteAsync(int id);
        Task<List<string>> GetGroupsAsync();
    }
}