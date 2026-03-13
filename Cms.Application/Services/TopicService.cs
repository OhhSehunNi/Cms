using Cms.Application.DTOs;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cms.Application.Services
{
    /// <summary>
    /// 专题服务实现类，用于专题相关的业务逻辑
    /// </summary>
    public class TopicService : ITopicService
    {
        private readonly CmsDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public TopicService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 根据 ID 获取专题
        /// </summary>
        /// <param name="id">专题 ID</param>
        /// <returns>专题 DTO</returns>
        public async Task<TopicDto> GetByIdAsync(int id)
        {
            var topic = await _dbContext.CmsTopics
                .Include(t => t.TopicArticles).ThenInclude(ta => ta.Article)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (topic == null)
                return null;

            return MapToDto(topic);
        }

        /// <summary>
        /// 根据 Slug 获取专题
        /// </summary>
        /// <param name="slug">专题 Slug</param>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>专题 DTO</returns>
        public async Task<TopicDto> GetBySlugAsync(string slug, int websiteId)
        {
            var topic = await _dbContext.CmsTopics
                .Include(t => t.TopicArticles).ThenInclude(ta => ta.Article)
                .FirstOrDefaultAsync(t => t.Slug == slug && t.WebsiteId == websiteId && !t.IsDeleted);

            if (topic == null)
                return null;

            return MapToDto(topic);
        }

        /// <summary>
        /// 获取专题列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>专题 DTO 列表</returns>
        public async Task<List<TopicDto>> GetListAsync(int page, int pageSize, string? keyword = null, int websiteId = 1)
        {
            IQueryable<CmsTopic> query = _dbContext.CmsTopics
                .Where(t => t.WebsiteId == websiteId && !t.IsDeleted);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.Name.Contains(keyword) || t.Slug.Contains(keyword) || t.Description.Contains(keyword));
            }

            var topics = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return topics.Select(MapToDto).ToList();
        }

        /// <summary>
        /// 创建专题
        /// </summary>
        /// <param name="topicDto">专题 DTO</param>
        /// <returns>创建后的专题 DTO</returns>
        public async Task<TopicDto> CreateAsync(TopicDto topicDto)
        {
            var topic = new CmsTopic
            {
                Name = topicDto.Name,
                Slug = topicDto.Slug,
                CoverImage = topicDto.CoverImage,
                Description = topicDto.Description,
                WebsiteId = topicDto.WebsiteId,
                IsEnabled = topicDto.IsEnabled,
                SeoTitle = topicDto.SeoTitle,
                SeoDescription = topicDto.SeoDescription,
                SeoKeywords = topicDto.SeoKeywords,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbContext.CmsTopics.Add(topic);
            await _dbContext.SaveChangesAsync();

            // 添加文章关联
            if (topicDto.ArticleIds != null && topicDto.ArticleIds.Count > 0)
            {
                await AddArticlesToTopicAsync(topic.Id, topicDto.ArticleIds);
            }

            return await GetByIdAsync(topic.Id);
        }

        /// <summary>
        /// 更新专题
        /// </summary>
        /// <param name="topicDto">专题 DTO</param>
        /// <returns>更新后的专题 DTO</returns>
        public async Task<TopicDto> UpdateAsync(TopicDto topicDto)
        {
            var topic = await _dbContext.CmsTopics.FindAsync(topicDto.Id);
            if (topic == null || topic.IsDeleted)
                throw new Exception("Topic not found");

            topic.Name = topicDto.Name;
            topic.Slug = topicDto.Slug;
            topic.CoverImage = topicDto.CoverImage;
            topic.Description = topicDto.Description;
            topic.IsEnabled = topicDto.IsEnabled;
            topic.SeoTitle = topicDto.SeoTitle;
            topic.SeoDescription = topicDto.SeoDescription;
            topic.SeoKeywords = topicDto.SeoKeywords;
            topic.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            // 更新文章关联
            await RemoveArticlesFromTopicAsync(topic.Id);
            if (topicDto.ArticleIds != null && topicDto.ArticleIds.Count > 0)
            {
                await AddArticlesToTopicAsync(topic.Id, topicDto.ArticleIds);
            }

            return await GetByIdAsync(topic.Id);
        }

        /// <summary>
        /// 删除专题
        /// </summary>
        /// <param name="id">专题 ID</param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var topic = await _dbContext.CmsTopics.FindAsync(id);
            if (topic != null)
            {
                topic.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 获取专题文章
        /// </summary>
        /// <param name="topicId">专题 ID</param>
        /// <param name="websiteId">网站 ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>文章 DTO 列表</returns>
        public async Task<List<ArticleDto>> GetTopicArticlesAsync(int topicId, int websiteId, int page, int pageSize)
        {
            var articles = await _dbContext.CmsTopicArticles
                .Include(ta => ta.Article)
                .ThenInclude(a => a.Channel)
                .Where(ta => ta.TopicId == topicId && ta.Article.WebsiteId == websiteId && ta.Article.Status == "Published" && !ta.Article.IsDeleted)
                .OrderByDescending(ta => ta.SortOrder)
                .ThenByDescending(ta => ta.Article.PublishTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ta => ta.Article)
                .ToListAsync();

            return articles.Select(a => new ArticleDto
            {
                Id = a.Id,
                Title = a.Title,
                SubTitle = a.SubTitle,
                Summary = a.Summary,
                CoverImage = a.CoverImage,
                VideoUrl = a.VideoUrl,
                ChannelId = a.ChannelId,
                ChannelName = a.Channel?.Name,
                ChannelSlug = a.Channel?.Slug,
                Author = a.Author,
                Source = a.Source,
                PublishTime = a.PublishTime,
                Status = a.Status,
                IsTop = a.IsTop,
                IsRecommended = a.IsRecommended,
                IsHeadline = a.IsHeadline,
                SortOrder = a.SortOrder,
                SeoTitle = a.SeoTitle,
                SeoDescription = a.SeoDescription,
                SeoKeywords = a.SeoKeywords,
                Slug = a.Slug,
                ViewCount = a.ViewCount
            }).ToList();
        }

        /// <summary>
        /// 为专题添加文章
        /// </summary>
        /// <param name="topicId">专题 ID</param>
        /// <param name="articleIds">文章 ID 列表</param>
        /// <returns></returns>
        public async Task AddArticlesToTopicAsync(int topicId, List<int> articleIds)
        {
            foreach (var articleId in articleIds)
            {
                var topicArticle = new CmsTopicArticle
                {
                    TopicId = topicId,
                    ArticleId = articleId,
                    SortOrder = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _dbContext.CmsTopicArticles.Add(topicArticle);
            }
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 从专题中移除文章
        /// </summary>
        /// <param name="topicId">专题 ID</param>
        /// <returns></returns>
        public async Task RemoveArticlesFromTopicAsync(int topicId)
        {
            var topicArticles = await _dbContext.CmsTopicArticles
                .Where(ta => ta.TopicId == topicId)
                .ToListAsync();
            _dbContext.CmsTopicArticles.RemoveRange(topicArticles);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 将实体映射为 DTO
        /// </summary>
        /// <param name="topic">专题实体</param>
        /// <returns>专题 DTO</returns>
        private TopicDto MapToDto(CmsTopic topic)
        {
            return new TopicDto
            {
                Id = topic.Id,
                Name = topic.Name,
                Slug = topic.Slug,
                CoverImage = topic.CoverImage,
                Description = topic.Description,
                WebsiteId = topic.WebsiteId,
                IsEnabled = topic.IsEnabled,
                SeoTitle = topic.SeoTitle,
                SeoDescription = topic.SeoDescription,
                SeoKeywords = topic.SeoKeywords,
                ArticleCount = topic.TopicArticles.Count,
                ArticleIds = topic.TopicArticles.Select(ta => ta.ArticleId).ToList()
            };
        }
    }
}