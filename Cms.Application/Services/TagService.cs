using Cms.Application.DTOs;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cms.Application.Services
{
    /// <summary>
    /// 标签服务实现类，用于标签相关的业务逻辑
    /// </summary>
    public class TagService : ITagService
    {
        private readonly CmsDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public TagService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 根据 ID 获取标签
        /// </summary>
        /// <param name="id">标签 ID</param>
        /// <returns>标签 DTO</returns>
        public async Task<TagDto> GetByIdAsync(int id)
        {
            var tag = await _dbContext.CmsTags
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (tag == null)
                return null;

            return MapToDto(tag);
        }

        /// <summary>
        /// 获取标签列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>标签 DTO 列表</returns>
        public async Task<List<TagDto>> GetListAsync(int page, int pageSize, string? keyword = null, int websiteId = 1)
        {
            IQueryable<CmsTag> query = _dbContext.CmsTags
                .Where(t => t.WebsiteId == websiteId && !t.IsDeleted);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.Name.Contains(keyword) || t.Slug.Contains(keyword));
            }

            var tags = await query
                .OrderBy(t => t.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return tags.Select(MapToDto).ToList();
        }

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="tagDto">标签 DTO</param>
        /// <returns>创建后的标签 DTO</returns>
        public async Task<TagDto> CreateAsync(TagDto tagDto)
        {
            var tag = new CmsTag
            {
                Name = tagDto.Name,
                Slug = tagDto.Slug,
                WebsiteId = tagDto.WebsiteId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbContext.CmsTags.Add(tag);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(tag.Id);
        }

        /// <summary>
        /// 更新标签
        /// </summary>
        /// <param name="tagDto">标签 DTO</param>
        /// <returns>更新后的标签 DTO</returns>
        public async Task<TagDto> UpdateAsync(TagDto tagDto)
        {
            var tag = await _dbContext.CmsTags.FindAsync(tagDto.Id);
            if (tag == null || tag.IsDeleted)
                throw new Exception("Tag not found");

            tag.Name = tagDto.Name;
            tag.Slug = tagDto.Slug;
            tag.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(tag.Id);
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="id">标签 ID</param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var tag = await _dbContext.CmsTags.FindAsync(id);
            if (tag != null)
            {
                tag.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>标签名称列表</returns>
        public async Task<List<string>> GetAllTagsAsync(int websiteId)
        {
            var tags = await _dbContext.CmsTags
                .Where(t => t.WebsiteId == websiteId && !t.IsDeleted)
                .OrderBy(t => t.Name)
                .Select(t => t.Name)
                .ToListAsync();

            return tags;
        }

        /// <summary>
        /// 获取文章标签
        /// </summary>
        /// <param name="articleId">文章 ID</param>
        /// <returns>标签名称列表</returns>
        public async Task<List<string>> GetArticleTagsAsync(int articleId)
        {
            var tags = await _dbContext.CmsArticleTags
                .Where(at => at.ArticleId == articleId && !at.IsDeleted)
                .Include(at => at.Tag)
                .Where(at => !at.Tag.IsDeleted)
                .Select(at => at.Tag.Name)
                .ToListAsync();

            return tags;
        }

        /// <summary>
        /// 为文章添加标签
        /// </summary>
        /// <param name="articleId">文章 ID</param>
        /// <param name="tags">标签名称列表</param>
        /// <returns></returns>
        public async Task AddTagsToArticleAsync(int articleId, List<string> tags)
        {
            // 先移除现有的标签关联
            var existingTags = await _dbContext.CmsArticleTags
                .Where(at => at.ArticleId == articleId)
                .ToListAsync();

            _dbContext.CmsArticleTags.RemoveRange(existingTags);

            // 添加新的标签关联
            foreach (var tagName in tags)
            {
                // 查找或创建标签
                var tag = await _dbContext.CmsTags
                    .FirstOrDefaultAsync(t => t.Name == tagName && t.WebsiteId == 1);

                if (tag == null)
                {
                    tag = new CmsTag
                    {
                        Name = tagName,
                        Slug = tagName.ToLower().Replace(" ", "-"),
                        WebsiteId = 1,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _dbContext.CmsTags.Add(tag);
                    await _dbContext.SaveChangesAsync();
                }

                // 创建文章标签关联
                var articleTag = new CmsArticleTag
                {
                    ArticleId = articleId,
                    TagId = tag.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _dbContext.CmsArticleTags.Add(articleTag);
            }

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 从文章中移除标签
        /// </summary>
        /// <param name="articleId">文章 ID</param>
        /// <returns></returns>
        public async Task RemoveTagsFromArticleAsync(int articleId)
        {
            var articleTags = await _dbContext.CmsArticleTags
                .Where(at => at.ArticleId == articleId)
                .ToListAsync();

            _dbContext.CmsArticleTags.RemoveRange(articleTags);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 获取带文章数量的标签
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <param name="limit">数量限制</param>
        /// <returns>标签名称、Slug 和文章数量的元组列表</returns>
        public async Task<List<(string Name, string Slug, int Count)>> GetTagsWithCountAsync(int websiteId, int limit = 20)
        {
            var tagsWithCount = await _dbContext.CmsTags
                .Where(t => t.WebsiteId == websiteId && !t.IsDeleted)
                .Select(t => new
                {
                    t.Name,
                    t.Slug,
                    Count = t.ArticleTags.Count(at => !at.IsDeleted)
                })
                .Where(t => t.Count > 0)
                .OrderByDescending(t => t.Count)
                .Take(limit)
                .ToListAsync();

            return tagsWithCount.Select(t => (t.Name, t.Slug, t.Count)).ToList();
        }

        /// <summary>
        /// 将实体映射为 DTO
        /// </summary>
        /// <param name="tag">标签实体</param>
        /// <returns>标签 DTO</returns>
        private TagDto MapToDto(CmsTag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Slug = tag.Slug,
                WebsiteId = tag.WebsiteId,
                ArticleCount = tag.ArticleTags.Count(at => !at.IsDeleted)
            };
        }
    }
}