using Cms.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cms.Application.Services
{
    /// <summary>
    /// 专题服务接口，用于专题相关的业务逻辑
    /// </summary>
    public interface ITopicService
    {
        /// <summary>
        /// 根据 ID 获取专题
        /// </summary>
        /// <param name="id">专题 ID</param>
        /// <returns>专题 DTO</returns>
        Task<TopicDto> GetByIdAsync(int id);

        /// <summary>
        /// 根据 slug 获取专题
        /// </summary>
        /// <param name="slug">专题 slug</param>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>专题 DTO</returns>
        Task<TopicDto> GetBySlugAsync(string slug, int websiteId);

        /// <summary>
        /// 获取专题列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>专题 DTO 列表</returns>
        Task<List<TopicDto>> GetListAsync(int page, int pageSize, string? keyword = null, int websiteId = 1);

        /// <summary>
        /// 创建专题
        /// </summary>
        /// <param name="topicDto">专题 DTO</param>
        /// <returns>创建后的专题 DTO</returns>
        Task<TopicDto> CreateAsync(TopicDto topicDto);

        /// <summary>
        /// 更新专题
        /// </summary>
        /// <param name="topicDto">专题 DTO</param>
        /// <returns>更新后的专题 DTO</returns>
        Task<TopicDto> UpdateAsync(TopicDto topicDto);

        /// <summary>
        /// 删除专题
        /// </summary>
        /// <param name="id">专题 ID</param>
        /// <returns></returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// 获取专题文章
        /// </summary>
        /// <param name="topicId">专题 ID</param>
        /// <param name="websiteId">网站 ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>文章 DTO 列表</returns>
        Task<List<ArticleDto>> GetTopicArticlesAsync(int topicId, int websiteId, int page, int pageSize);

        /// <summary>
        /// 为专题添加文章
        /// </summary>
        /// <param name="topicId">专题 ID</param>
        /// <param name="articleIds">文章 ID 列表</param>
        /// <returns></returns>
        Task AddArticlesToTopicAsync(int topicId, List<int> articleIds);

        /// <summary>
        /// 从专题中移除文章
        /// </summary>
        /// <param name="topicId">专题 ID</param>
        /// <returns></returns>
        Task RemoveArticlesFromTopicAsync(int topicId);
    }
}