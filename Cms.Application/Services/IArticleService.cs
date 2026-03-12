using Cms.Application.DTOs;

namespace Cms.Application.Services
{
    public interface IArticleService
    {
        Task<ArticleDto> GetByIdAsync(int id);
        Task<List<ArticleDto>> GetListAsync(int page, int pageSize, string? keyword = null, int? channelId = null);
        Task<ArticleDto> CreateAsync(ArticleDto articleDto);
        Task<ArticleDto> UpdateAsync(ArticleDto articleDto);
        Task DeleteAsync(int id);
        Task PublishAsync(int id);
        Task UnpublishAsync(int id);
        Task IncrementViewCountAsync(int id);
    }
}