using Cms.Application.Services.Dtos;

namespace Cms.Application.Services
{
    /// <summary>
    /// 文章服务接口，定义文章相关的业务逻辑
    /// </summary>
    public interface IArticleService
    {
        /// <summary>
        /// 根据 ID 获取文章
        /// </summary>
        /// <param name="id">文章 ID</param>
        /// <returns>文章 DTO</returns>
        Task<ArticleDto> GetByIdAsync(int id);

        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <param name="channelId">栏目 ID</param>
        /// <param name="status">状态</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="isTop">是否置顶</param>
        /// <param name="isRecommended">是否推荐</param>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>文章 DTO 列表</returns>
        Task<List<ArticleDto>> GetListAsync(int page, int pageSize, string? keyword = null, int? channelId = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null, bool? isTop = null, bool? isRecommended = null, int websiteId = 1);

        /// <summary>
        /// 创建文章
        /// </summary>
        /// <param name="articleDto">文章 DTO</param>
        /// <returns>创建后的文章 DTO</returns>
        Task<ArticleDto> CreateAsync(ArticleDto articleDto);

        /// <summary>
        /// 更新文章
        /// </summary>
        /// <param name="articleDto">文章 DTO</param>
        /// <returns>更新后的文章 DTO</returns>
        Task<ArticleDto> UpdateAsync(ArticleDto articleDto);

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="id">文章 ID</param>
        /// <returns></returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// 发布文章
        /// </summary>
        /// <param name="id">文章 ID</param>
        /// <returns>发布后的文章 DTO</returns>
        Task<ArticleDto> PublishAsync(int id);

        /// <summary>
        /// 下线文章
        /// </summary>
        /// <param name="id">文章 ID</param>
        /// <returns>下线后的文章 DTO</returns>
        Task<ArticleDto> OfflineAsync(int id);

        /// <summary>
        /// 增加文章浏览量
        /// </summary>
        /// <param name="id">文章 ID</param>
        /// <returns></returns>
        Task IncrementViewCountAsync(int id);

        /// <summary>
        /// 获取头条文章
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <param name="limit">数量限制</param>
        /// <returns>文章 DTO 列表</returns>
        Task<List<ArticleDto>> GetHeadlineArticlesAsync(int websiteId, int limit = 5);

        /// <summary>
        /// 获取热门文章
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <param name="limit">数量限制</param>
        /// <returns>文章 DTO 列表</returns>
        Task<List<ArticleDto>> GetHotArticlesAsync(int websiteId, int limit = 10);
    }
}