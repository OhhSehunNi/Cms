using Cms.Application.Services.Dtos;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Cms.Application.Services
{
    /// <summary>
    /// 文章服务实现类，用于文章相关的业务逻辑
    /// </summary>
    public class ArticleService : IArticleService
    {
        private readonly CmsDbContext _dbContext;
        private readonly IHtmlSanitizerService _htmlSanitizerService;
        private readonly ICacheService _cacheService;
        private readonly ArticleDapperService _articleDapperService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="htmlSanitizerService">HTML 清洗服务</param>
        /// <param name="cacheService">缓存服务</param>
        /// <param name="configuration">配置对象</param>
        public ArticleService(CmsDbContext dbContext, IHtmlSanitizerService htmlSanitizerService, ICacheService cacheService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _htmlSanitizerService = htmlSanitizerService;
            _cacheService = cacheService;
            _articleDapperService = new ArticleDapperService(configuration);
        }

        /// <summary>
        /// 根据 ID 获取文章
        /// </summary>
        /// <param name="id">文章 ID</param>
        /// <returns>文章 DTO</returns>
        public async Task<ArticleDto> GetByIdAsync(int id)
        {
            // 尝试从缓存获取
            string cacheKey = $"website:1:article:{id}";
            var cachedArticle = _cacheService.Get<ArticleDto>(cacheKey);
            if (cachedArticle != null)
                return cachedArticle;

            var article = await _dbContext.CmsArticles
                .Include(a => a.Channel)
                .Include(a => a.Content)
                .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
                return null;

            var articleDto = MapToDto(article);

            // 缓存文章详情
            _cacheService.Set(cacheKey, articleDto, TimeSpan.FromHours(1));

            return articleDto;
        }

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
        public async Task<List<ArticleDto>> GetListAsync(int page, int pageSize, string? keyword = null, int? channelId = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null, bool? isTop = null, bool? isRecommended = null, int websiteId = 1)
        {
            // 尝试从缓存获取
            string cacheKey = $"website:{websiteId}:channel:{channelId ?? 0}:status:{status ?? string.Empty}:start:{startDate?.ToString("yyyy-MM-dd") ?? string.Empty}:end:{endDate?.ToString("yyyy-MM-dd") ?? string.Empty}:top:{isTop ?? false}:recommended:{isRecommended ?? false}:list:{page}:{pageSize}:{keyword ?? string.Empty}";
            var cachedList = _cacheService.Get<List<ArticleDto>>(cacheKey);
            if (cachedList != null)
                return cachedList;

            // 使用 Dapper 进行高性能查询
            var articles = await _articleDapperService.GetListAsync(page, pageSize, keyword, channelId, status, startDate, endDate, isTop, isRecommended, websiteId);

            // 缓存文章列表
            _cacheService.Set(cacheKey, articles, TimeSpan.FromMinutes(30));

            return articles;
        }

        /// <summary>
        /// 创建文章
        /// </summary>
        /// <param name="articleDto">文章 DTO</param>
        /// <returns>创建后的文章 DTO</returns>
        public async Task<ArticleDto> CreateAsync(ArticleDto articleDto)
        {
            // 处理富文本内容
            var sanitizedHtml = _htmlSanitizerService.SanitizeHtml(articleDto.HtmlContent);
            var plainText = _htmlSanitizerService.ExtractPlainText(sanitizedHtml);
            var wordCount = _htmlSanitizerService.CalculateWordCount(sanitizedHtml);

            var article = MapToEntity(articleDto);
            article.Create();
            article.Content.HtmlContent = sanitizedHtml;
            article.Content.TextContent = plainText;
            article.Content.WordCount = wordCount;

            _dbContext.CmsArticles.Add(article);
            await _dbContext.SaveChangesAsync();

            // 清除相关缓存
            ClearArticleCache(article.WebsiteId, article.ChannelId);

            return await GetByIdAsync(article.Id);
        }

        /// <summary>
        /// 更新文章
        /// </summary>
        /// <param name="articleDto">文章 DTO</param>
        /// <returns>更新后的文章 DTO</returns>
        public async Task<ArticleDto> UpdateAsync(ArticleDto articleDto)
        {
            var article = await _dbContext.CmsArticles
                .Include(a => a.Content)
                .Include(a => a.ArticleTags)
                .FirstOrDefaultAsync(a => a.Id == articleDto.Id);

            if (article == null)
                throw new Exception("文章不存在");

            // 处理富文本内容
            var sanitizedHtml = _htmlSanitizerService.SanitizeHtml(articleDto.HtmlContent);
            var plainText = _htmlSanitizerService.ExtractPlainText(sanitizedHtml);
            var wordCount = _htmlSanitizerService.CalculateWordCount(sanitizedHtml);

            UpdateEntityFromDto(article, articleDto);
            article.Update();
            article.Content.HtmlContent = sanitizedHtml;
            article.Content.TextContent = plainText;
            article.Content.WordCount = wordCount;

            await _dbContext.SaveChangesAsync();

            // 清除相关缓存
            ClearArticleCache(article.WebsiteId, article.ChannelId);
            _cacheService.Remove($"website:{article.WebsiteId}:article:{article.Id}");

            return await GetByIdAsync(article.Id);
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="id">文章 ID</param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var article = await _dbContext.CmsArticles
                .Include(a => a.Channel)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
                throw new Exception("文章不存在");

            article.Delete();
            await _dbContext.SaveChangesAsync();

            // 清除相关缓存
            ClearArticleCache(article.WebsiteId, article.ChannelId);
            _cacheService.Remove($"website:{article.WebsiteId}:article:{article.Id}");
        }

        /// <summary>
        /// 发布文章
        /// </summary>
        /// <param name="id">文章 ID</param>
        /// <returns></returns>
        public async Task<ArticleDto> PublishAsync(int id)
        {
            var article = await _dbContext.CmsArticles
                .Include(a => a.Content)
                .Include(a => a.Channel)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
                throw new Exception("文章不存在");

            // 检查栏目是否启用
            if (!article.Channel.IsEnabled)
                throw new Exception("栏目已禁用，无法发布文章");

            // 检查是否有正文
            if (!article.CanPublish())
                throw new Exception("正文不能为空");

            article.Publish();
            await _dbContext.SaveChangesAsync();

            // 清除相关缓存
            ClearArticleCache(article.WebsiteId, article.ChannelId);
            _cacheService.Remove($"website:{article.WebsiteId}:article:{article.Id}");

            return await GetByIdAsync(article.Id);
        }

        /// <summary>
        /// 下线文章
        /// </summary>
        /// <param name="id">文章 ID</param>
        /// <returns></returns>
        public async Task<ArticleDto> OfflineAsync(int id)
        {
            var article = await _dbContext.CmsArticles
                .Include(a => a.Channel)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
                throw new Exception("文章不存在");

            article.Offline();
            await _dbContext.SaveChangesAsync();

            // 清除相关缓存
            ClearArticleCache(article.WebsiteId, article.ChannelId);
            _cacheService.Remove($"website:{article.WebsiteId}:article:{article.Id}");

            return await GetByIdAsync(article.Id);
        }

        /// <summary>
        /// 增加文章浏览量
        /// </summary>
        /// <param name="id">文章 ID</param>
        /// <returns></returns>
        public async Task IncrementViewCountAsync(int id)
        {
            var article = await _dbContext.CmsArticles.FindAsync(id);
            if (article != null)
            {
                article.ViewCount++;
                await _dbContext.SaveChangesAsync();

                // 清除缓存
                _cacheService.Remove($"website:{article.WebsiteId}:article:{article.Id}");
            }
        }

        /// <summary>
        /// 获取头条文章
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <param name="limit">数量限制</param>
        /// <returns>文章 DTO 列表</returns>
        public async Task<List<ArticleDto>> GetHeadlineArticlesAsync(int websiteId, int limit = 5)
        {
            string cacheKey = $"website:{websiteId}:headline:{limit}";
            var cachedArticles = _cacheService.Get<List<ArticleDto>>(cacheKey);
            if (cachedArticles != null)
                return cachedArticles;

            var articles = await _dbContext.CmsArticles
                .Include(a => a.Channel)
                .Where(a => a.WebsiteId == websiteId && a.IsHeadline && a.Status == ArticleStatus.Published && !a.IsDeleted)
                .OrderByDescending(a => a.SortOrder)
                .ThenByDescending(a => a.PublishTime)
                .Take(limit)
                .ToListAsync();

            var articleDtos = articles.Select(MapToDto).ToList();

            _cacheService.Set(cacheKey, articleDtos, TimeSpan.FromMinutes(30));

            return articleDtos;
        }

        /// <summary>
        /// 获取热门文章
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <param name="limit">数量限制</param>
        /// <returns>文章 DTO 列表</returns>
        public async Task<List<ArticleDto>> GetHotArticlesAsync(int websiteId, int limit = 10)
        {
            string cacheKey = $"website:{websiteId}:hot:{limit}";
            var cachedArticles = _cacheService.Get<List<ArticleDto>>(cacheKey);
            if (cachedArticles != null)
                return cachedArticles;

            var articles = await _dbContext.CmsArticles
                .Include(a => a.Channel)
                .Where(a => a.WebsiteId == websiteId && a.Status == ArticleStatus.Published && !a.IsDeleted)
                .OrderByDescending(a => a.ViewCount)
                .ThenByDescending(a => a.PublishTime)
                .Take(limit)
                .ToListAsync();

            var articleDtos = articles.Select(MapToDto).ToList();

            _cacheService.Set(cacheKey, articleDtos, TimeSpan.FromMinutes(30));

            return articleDtos;
        }

        /// <summary>
        /// 清除文章相关缓存
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <param name="channelId">栏目 ID</param>
        private void ClearArticleCache(int websiteId, int channelId)
        {
            // 清除栏目列表缓存
            _cacheService.Remove($"website:{websiteId}:channel:{channelId}:list:*");
            // 清除首页缓存
            _cacheService.Remove($"website:{websiteId}:home");
            // 清除头条缓存
            _cacheService.Remove($"website:{websiteId}:headline:*");
            // 清除热门文章缓存
            _cacheService.Remove($"website:{websiteId}:hot:*");
        }

        /// <summary>
        /// 将实体映射为 DTO
        /// </summary>
        /// <param name="article">文章实体</param>
        /// <returns>文章 DTO</returns>
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
                ChannelSlug = article.Channel?.Slug,
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

        /// <summary>
        /// 将 DTO 映射为实体
        /// </summary>
        /// <param name="articleDto">文章 DTO</param>
        /// <returns>文章实体</returns>
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
                    TextContent = articleDto.TextContent,
                    WordCount = 0
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

        /// <summary>
        /// 从 DTO 更新实体
        /// </summary>
        /// <param name="article">文章实体</param>
        /// <param name="articleDto">文章 DTO</param>
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