using Cms.Application.DTOs;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cms.Application.Services
{
    public class RecommendService : IRecommendService
    {
        private readonly CmsDbContext _dbContext;

        public RecommendService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RecommendSlotDto> GetSlotByIdAsync(int id)
        {
            var slot = await _dbContext.CmsRecommendSlots
                .Include(s => s.RecommendItems)
                .ThenInclude(i => i.Article)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (slot == null)
                return null;

            return MapSlotToDto(slot);
        }

        public async Task<RecommendSlotDto> GetSlotByCodeAsync(string code)
        {
            var slot = await _dbContext.CmsRecommendSlots
                .Include(s => s.RecommendItems)
                .ThenInclude(i => i.Article)
                .FirstOrDefaultAsync(s => s.Code == code && s.IsEnabled && !s.IsDeleted);

            if (slot == null)
                return null;

            return MapSlotToDto(slot);
        }

        public async Task<List<RecommendSlotDto>> GetSlotListAsync(int page, int pageSize, string keyword = null)
        {
            var query = _dbContext.CmsRecommendSlots.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.Name.Contains(keyword) || s.Code.Contains(keyword));
            }

            var slots = await query
                .OrderBy(s => s.SortOrder)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return slots.Select(MapSlotToDto).ToList();
        }

        public async Task<RecommendSlotDto> CreateSlotAsync(RecommendSlotDto slotDto)
        {
            var slot = MapSlotToEntity(slotDto);
            slot.CreatedAt = DateTime.Now;
            slot.UpdatedAt = DateTime.Now;

            _dbContext.CmsRecommendSlots.Add(slot);
            await _dbContext.SaveChangesAsync();

            return await GetSlotByIdAsync(slot.Id);
        }

        public async Task<RecommendSlotDto> UpdateSlotAsync(RecommendSlotDto slotDto)
        {
            var slot = await _dbContext.CmsRecommendSlots.FindAsync(slotDto.Id);
            if (slot == null)
                throw new Exception("RecommendSlot not found");

            UpdateSlotEntityFromDto(slot, slotDto);
            slot.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return await GetSlotByIdAsync(slot.Id);
        }

        public async Task DeleteSlotAsync(int id)
        {
            var slot = await _dbContext.CmsRecommendSlots.FindAsync(id);
            if (slot != null)
            {
                slot.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<RecommendItemDto> AddItemAsync(RecommendItemDto itemDto)
        {
            var item = MapItemToEntity(itemDto);
            item.CreatedAt = DateTime.Now;
            item.UpdatedAt = DateTime.Now;

            _dbContext.CmsRecommendItems.Add(item);
            await _dbContext.SaveChangesAsync();

            return await GetItemByIdAsync(item.Id);
        }

        public async Task<RecommendItemDto> UpdateItemAsync(RecommendItemDto itemDto)
        {
            var item = await _dbContext.CmsRecommendItems.FindAsync(itemDto.Id);
            if (item == null)
                throw new Exception("RecommendItem not found");

            UpdateItemEntityFromDto(item, itemDto);
            item.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return await GetItemByIdAsync(item.Id);
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await _dbContext.CmsRecommendItems.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<ArticleDto>> GetRecommendArticlesAsync(string code, int count = 10)
        {
            var slot = await _dbContext.CmsRecommendSlots
                .Include(s => s.RecommendItems)
                .ThenInclude(i => i.Article)
                .ThenInclude(a => a.Channel)
                .FirstOrDefaultAsync(s => s.Code == code && s.IsEnabled && !s.IsDeleted);

            if (slot == null)
                return new List<ArticleDto>();

            var now = DateTime.Now;
            var items = slot.RecommendItems
                .Where(i => !i.IsDeleted && (!i.StartTime.HasValue || i.StartTime <= now) && (!i.EndTime.HasValue || i.EndTime >= now))
                .OrderBy(i => i.SortOrder)
                .Take(count)
                .Select(i => i.Article)
                .ToList();

            return items.Select(MapArticleToDto).ToList();
        }

        private async Task<RecommendItemDto> GetItemByIdAsync(int id)
        {
            var item = await _dbContext.CmsRecommendItems
                .Include(i => i.Article)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return null;

            return MapItemToDto(item);
        }

        private RecommendSlotDto MapSlotToDto(CmsRecommendSlot slot)
        {
            return new RecommendSlotDto
            {
                Id = slot.Id,
                Name = slot.Name,
                Code = slot.Code,
                Type = slot.Type,
                SortOrder = slot.SortOrder,
                IsEnabled = slot.IsEnabled,
                RecommendItems = slot.RecommendItems.Select(MapItemToDto).ToList()
            };
        }

        private CmsRecommendSlot MapSlotToEntity(RecommendSlotDto slotDto)
        {
            return new CmsRecommendSlot
            {
                Name = slotDto.Name,
                Code = slotDto.Code,
                Type = slotDto.Type,
                SortOrder = slotDto.SortOrder,
                IsEnabled = slotDto.IsEnabled
            };
        }

        private void UpdateSlotEntityFromDto(CmsRecommendSlot slot, RecommendSlotDto slotDto)
        {
            slot.Name = slotDto.Name;
            slot.Code = slotDto.Code;
            slot.Type = slotDto.Type;
            slot.SortOrder = slotDto.SortOrder;
            slot.IsEnabled = slotDto.IsEnabled;
        }

        private RecommendItemDto MapItemToDto(CmsRecommendItem item)
        {
            return new RecommendItemDto
            {
                Id = item.Id,
                SlotId = item.SlotId,
                ArticleId = item.ArticleId,
                ArticleTitle = item.Article?.Title,
                ArticleCoverImage = item.Article?.CoverImage,
                SortOrder = item.SortOrder,
                StartTime = item.StartTime,
                EndTime = item.EndTime
            };
        }

        private CmsRecommendItem MapItemToEntity(RecommendItemDto itemDto)
        {
            return new CmsRecommendItem
            {
                SlotId = itemDto.SlotId,
                ArticleId = itemDto.ArticleId,
                SortOrder = itemDto.SortOrder,
                StartTime = itemDto.StartTime,
                EndTime = itemDto.EndTime
            };
        }

        private void UpdateItemEntityFromDto(CmsRecommendItem item, RecommendItemDto itemDto)
        {
            item.SlotId = itemDto.SlotId;
            item.ArticleId = itemDto.ArticleId;
            item.SortOrder = itemDto.SortOrder;
            item.StartTime = itemDto.StartTime;
            item.EndTime = itemDto.EndTime;
        }

        private ArticleDto MapArticleToDto(CmsArticle article)
        {
            return new ArticleDto
            {
                Id = article.Id,
                Title = article.Title,
                SubTitle = article.SubTitle,
                Summary = article.Summary,
                CoverImage = article.CoverImage,
                VideoUrl = article.VideoUrl,
                ChannelId = article.ChannelId,
                ChannelName = article.Channel?.Name,
                Author = article.Author,
                Source = article.Source,
                PublishTime = article.PublishTime,
                Status = article.Status,
                IsTop = article.IsTop,
                IsRecommended = article.IsRecommended,
                IsHeadline = article.IsHeadline,
                SortOrder = article.SortOrder,
                SeoTitle = article.SeoTitle,
                SeoDescription = article.SeoDescription,
                SeoKeywords = article.SeoKeywords,
                Slug = article.Slug,
                ViewCount = article.ViewCount
            };
        }
    }
}