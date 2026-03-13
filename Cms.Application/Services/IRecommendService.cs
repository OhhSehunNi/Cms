using Cms.Application.DTOs;

namespace Cms.Application.Services
{
    /// <summary>
    /// 推荐服务接口，用于推荐位相关的业务逻辑
    /// </summary>
    public interface IRecommendService
    {
        /// <summary>
        /// 根据 ID 获取推荐位
        /// </summary>
        /// <param name="id">推荐位 ID</param>
        /// <returns>推荐位 DTO</returns>
        Task<RecommendSlotDto> GetSlotByIdAsync(int id);

        /// <summary>
        /// 根据代码获取推荐位
        /// </summary>
        /// <param name="code">推荐位代码</param>
        /// <returns>推荐位 DTO</returns>
        Task<RecommendSlotDto> GetSlotByCodeAsync(string code);

        /// <summary>
        /// 获取推荐位列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <returns>推荐位 DTO 列表</returns>
        Task<List<RecommendSlotDto>> GetSlotListAsync(int page, int pageSize, string? keyword = null);

        /// <summary>
        /// 创建推荐位
        /// </summary>
        /// <param name="slotDto">推荐位 DTO</param>
        /// <returns>创建后的推荐位 DTO</returns>
        Task<RecommendSlotDto> CreateSlotAsync(RecommendSlotDto slotDto);

        /// <summary>
        /// 更新推荐位
        /// </summary>
        /// <param name="slotDto">推荐位 DTO</param>
        /// <returns>更新后的推荐位 DTO</returns>
        Task<RecommendSlotDto> UpdateSlotAsync(RecommendSlotDto slotDto);

        /// <summary>
        /// 删除推荐位
        /// </summary>
        /// <param name="id">推荐位 ID</param>
        /// <returns></returns>
        Task DeleteSlotAsync(int id);

        /// <summary>
        /// 添加推荐位项目
        /// </summary>
        /// <param name="itemDto">推荐位项目 DTO</param>
        /// <returns>添加后的推荐位项目 DTO</returns>
        Task<RecommendItemDto> AddItemAsync(RecommendItemDto itemDto);

        /// <summary>
        /// 更新推荐位项目
        /// </summary>
        /// <param name="itemDto">推荐位项目 DTO</param>
        /// <returns>更新后的推荐位项目 DTO</returns>
        Task<RecommendItemDto> UpdateItemAsync(RecommendItemDto itemDto);

        /// <summary>
        /// 删除推荐位项目
        /// </summary>
        /// <param name="id">推荐位项目 ID</param>
        /// <returns></returns>
        Task DeleteItemAsync(int id);

        /// <summary>
        /// 获取推荐文章
        /// </summary>
        /// <param name="code">推荐位代码</param>
        /// <param name="count">数量限制</param>
        /// <returns>文章 DTO 列表</returns>
        Task<List<ArticleDto>> GetRecommendArticlesAsync(string code, int count = 10);
    }
}