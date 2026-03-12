using Cms.Application.DTOs;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cms.Application.Services
{
    public class ArticleService : IArticleService
    {
        private readonly CmsDbContext _dbContext;

        public ArticleService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ArticleDto> GetByIdAsync(int id)
        {
            var article = await _dbContext.CmsArticles
                .Include(a => a.Channel)
                .Include(a => a.Content)
                .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
                return null;

            return MapToDto(article);
        }

        public async Task<List<ArticleDto>> GetListAsync(int page, int pageSize, string keyword = null, int? channelId = null)
        {
            IQueryable<CmsArticle> query = _dbContext.CmsArticles
                .Include(a => a.Channel)
                .Include(a => a.ArticleTags).ThenInclude(at => at.Tag);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.Title.Contains(keyword) || a.Summary.Contains(keyword));
            }

            if (channelId.HasValue)
            {
                query = query.Where(a => a.ChannelId == channelId.Value);
            }

            var articles = await query
                .OrderByDescending(a => a.IsTop)
                .ThenByDescending(a => a.PublishTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return articles.Select(MapToDto).ToList();
        }

        public async Task<ArticleDto> CreateAsync(ArticleDto articleDto)
        {
            var article = MapToEntity(articleDto);
            article.Status = "Draft";
            article.CreatedAt = DateTime.Now;
            article.UpdatedAt = DateTime.Now;

            _dbContext.CmsArticles.Add(article);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(article.Id);
        }

        public async Task<ArticleDto> UpdateAsync(ArticleDto articleDto)
        {
            var article = await _dbContext.CmsArticles
                .Include(a => a.Content)
                .Include(a => a.ArticleTags)
                .FirstOrDefaultAsync(a => a.Id == articleDto.Id);

            if (article == null)
                throw new Exception("Article not found");

            UpdateEntityFromDto(article, articleDto);
            article.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(article.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var article = await _dbContext.CmsArticles.FindAsync(id);
            if (article != null)
            {
                article.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task PublishAsync(int id)
        {
            var article = await _dbContext.CmsArticles.FindAsync(id);
            if (article != null)
            {
                article.Status = "Published";
                article.PublishTime = DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UnpublishAsync(int id)
        {
            var article = await _dbContext.CmsArticles.FindAsync(id);
            if (article != null)
            {
                article.Status = "Unpublished";
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task IncrementViewCountAsync(int id)
        {
            var article = await _dbContext.CmsArticles.FindAsync(id);
            if (article != null)
            {
                article.ViewCount++;
                await _dbContext.SaveChangesAsync();
            }
        }

        private ArticleDto MapToDto(CmsArticle article)
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
                ViewCount = article.ViewCount,
                HtmlContent = article.Content?.HtmlContent,
                TextContent = article.Content?.TextContent,
                TagIds = article.ArticleTags.Select(at => at.TagId).ToList(),
                TagNames = article.ArticleTags.Select(at => at.Tag.Name).ToList()
            };
        }

        private CmsArticle MapToEntity(ArticleDto articleDto)
        {
            var article = new CmsArticle
            {
                Title = articleDto.Title,
                SubTitle = articleDto.SubTitle,
                Summary = articleDto.Summary,
                CoverImage = articleDto.CoverImage,
                VideoUrl = articleDto.VideoUrl,
                ChannelId = articleDto.ChannelId,
                Author = articleDto.Author,
                Source = articleDto.Source,
                PublishTime = articleDto.PublishTime,
                Status = articleDto.Status,
                IsTop = articleDto.IsTop,
                IsRecommended = articleDto.IsRecommended,
                IsHeadline = articleDto.IsHeadline,
                SortOrder = articleDto.SortOrder,
                SeoTitle = articleDto.SeoTitle,
                SeoDescription = articleDto.SeoDescription,
                SeoKeywords = articleDto.SeoKeywords,
                Slug = articleDto.Slug,
                ViewCount = articleDto.ViewCount,
                Content = new CmsArticleContent
                {
                    HtmlContent = articleDto.HtmlContent,
                    TextContent = articleDto.TextContent
                }
            };

            if (articleDto.TagIds != null)
            {
                article.ArticleTags = articleDto.TagIds.Select(tagId => new CmsArticleTag
                {
                    TagId = tagId
                }).ToList();
            }

            return article;
        }

        private void UpdateEntityFromDto(CmsArticle article, ArticleDto articleDto)
        {
            article.Title = articleDto.Title;
            article.SubTitle = articleDto.SubTitle;
            article.Summary = articleDto.Summary;
            article.CoverImage = articleDto.CoverImage;
            article.VideoUrl = articleDto.VideoUrl;
            article.ChannelId = articleDto.ChannelId;
            article.Author = articleDto.Author;
            article.Source = articleDto.Source;
            article.PublishTime = articleDto.PublishTime;
            article.Status = articleDto.Status;
            article.IsTop = articleDto.IsTop;
            article.IsRecommended = articleDto.IsRecommended;
            article.IsHeadline = articleDto.IsHeadline;
            article.SortOrder = articleDto.SortOrder;
            article.SeoTitle = articleDto.SeoTitle;
            article.SeoDescription = articleDto.SeoDescription;
            article.SeoKeywords = articleDto.SeoKeywords;
            article.Slug = articleDto.Slug;

            if (article.Content == null)
            {
                article.Content = new CmsArticleContent();
            }
            article.Content.HtmlContent = articleDto.HtmlContent;
            article.Content.TextContent = articleDto.TextContent;

            // 更新标签
            article.ArticleTags.Clear();
            if (articleDto.TagIds != null)
            {
                article.ArticleTags = articleDto.TagIds.Select(tagId => new CmsArticleTag
                {
                    TagId = tagId
                }).ToList();
            }
        }
    }
}