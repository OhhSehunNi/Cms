using Cms.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cms.Application.Services
{
    /// <summary>
    /// 标签服务接口，用于标签相关的业务逻辑
    /// </summary>
    public interface ITagService
    {
        /// <summary>
        /// 根据 ID 获取标签
        /// </summary>
        /// <param name="id">标签 ID</param>
        /// <returns>标签 DTO</returns>
        Task<TagDto> GetByIdAsync(int id);

        /// <summary>
        /// 获取标签列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>标签 DTO 列表</returns>
        Task<List<TagDto>> GetListAsync(int page, int pageSize, string? keyword = null, int websiteId = 1);

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="tagDto">标签 DTO</param>
        /// <returns>创建后的标签 DTO</returns>
        Task<TagDto> CreateAsync(TagDto tagDto);

        /// <summary>
        /// 更新标签
        /// </summary>
        /// <param name="tagDto">标签 DTO</param>
        /// <returns>更新后的标签 DTO</returns>
        Task<TagDto> UpdateAsync(TagDto tagDto);

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="id">标签 ID</param>
        /// <returns></returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>标签名称列表</returns>
        Task<List<string>> GetAllTagsAsync(int websiteId);

        /// <summary>
        /// 获取文章标签
        /// </summary>
        /// <param name="articleId">文章 ID</param>
        /// <returns>标签名称列表</returns>
        Task<List<string>> GetArticleTagsAsync(int articleId);

        /// <summary>
        /// 为文章添加标签
        /// </summary>
        /// <param name="articleId">文章 ID</param>
        /// <param name="tags">标签列表</param>
        /// <returns></returns>
        Task AddTagsToArticleAsync(int articleId, List<string> tags);

        /// <summary>
        /// 从文章中移除标签
        /// </summary>
        /// <param name="articleId">文章 ID</param>
        /// <returns></returns>
        Task RemoveTagsFromArticleAsync(int articleId);

        /// <summary>
        /// 获取带有文章数量的标签
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <param name="limit">数量限制</param>
        /// <returns>标签名称、slug 和文章数量的元组列表</returns>
        Task<List<(string Name, string Slug, int Count)>> GetTagsWithCountAsync(int websiteId, int limit = 20);
    }
}