using Cms.Application.DTOs;

namespace Cms.Application.Services
{
    public interface IRecommendService
    {
        Task<RecommendSlotDto> GetSlotByIdAsync(int id);
        Task<RecommendSlotDto> GetSlotByCodeAsync(string code);
        Task<List<RecommendSlotDto>> GetSlotListAsync(int page, int pageSize, string? keyword = null);
        Task<RecommendSlotDto> CreateSlotAsync(RecommendSlotDto slotDto);
        Task<RecommendSlotDto> UpdateSlotAsync(RecommendSlotDto slotDto);
        Task DeleteSlotAsync(int id);
        Task<RecommendItemDto> AddItemAsync(RecommendItemDto itemDto);
        Task<RecommendItemDto> UpdateItemAsync(RecommendItemDto itemDto);
        Task DeleteItemAsync(int id);
        Task<List<ArticleDto>> GetRecommendArticlesAsync(string code, int count = 10);
    }
}