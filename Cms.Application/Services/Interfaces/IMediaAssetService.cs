using Cms.Application.Services.Dtos;

namespace Cms.Application.Services
{
    /// <summary>
    /// 媒体资源服务接口，用于媒体资源相关的业务逻辑
    /// </summary>
    public interface IMediaAssetService
    {
        /// <summary>
        /// 根据 ID 获取媒体资源
        /// </summary>
        /// <param name="id">资源 ID</param>
        /// <returns>媒体资源 DTO</returns>
        Task<MediaAssetDto> GetByIdAsync(int id);

        /// <summary>
        /// 获取媒体资源列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <param name="group">分组</param>
        /// <returns>媒体资源 DTO 列表</returns>
        Task<List<MediaAssetDto>> GetListAsync(int page, int pageSize, string? keyword = null, string? group = null);

        /// <summary>
        /// 上传媒体资源
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="fileData">文件数据</param>
        /// <param name="group">分组</param>
        /// <returns>上传后的媒体资源 DTO</returns>
        Task<MediaAssetDto> UploadAsync(string fileName, string contentType, long fileSize, byte[] fileData, string? group = null);

        /// <summary>
        /// 删除媒体资源
        /// </summary>
        /// <param name="id">资源 ID</param>
        /// <returns></returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// 获取分组列表
        /// </summary>
        /// <returns>分组名称列表</returns>
        Task<List<string>> GetGroupsAsync();
    }
}