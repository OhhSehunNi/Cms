using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cms.Web.ViewModels;
using Cms.Application.Services;

namespace Cms.Web.Controllers
{
    /// <summary>
    /// 文章控制器
    /// 处理文章相关的视图请求，包括文章详情页的展示
    /// </summary>
    public class ArticleController : Controller
    {
        /// <summary>
        /// 文章服务接口
        /// </summary>
        private readonly IArticleService _articleService;
        /// <summary>
        /// 网站服务接口
        /// </summary>
        private readonly IWebsiteService _websiteService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="articleService">文章服务实例</param>
        /// <param name="websiteService">网站服务实例</param>
        public ArticleController(IArticleService articleService, IWebsiteService websiteService)
        {
            _articleService = articleService;
            _websiteService = websiteService;
        }

        /// <summary>
        /// 文章详情页
        /// </summary>
        /// <param name="channelSlug">频道Slug</param>
        /// <param name="year">发布年份</param>
        /// <param name="month">发布月份</param>
        /// <param name="day">发布日期</param>
        /// <param name="id">文章ID</param>
        /// <returns>文章详情视图</returns>
        [Route("{channelSlug}/{year:int}/{month:int}/{day:int}/{id}.html")]
        [ResponseCache(Duration = 3600, VaryByHeader = "Host")]
        public async Task<IActionResult> Detail(string channelSlug, int year, int month, int day, int id)
        {
            // 从当前请求上下文中获取WebsiteId
            var websiteId = HttpContext.Items["WebsiteId"] as int? ?? 1;
            
            // 获取网站信息
            var website = await _websiteService.GetByIdAsync(websiteId);
            ViewBag.WebsiteName = website?.Name ?? "新闻网站";
            ViewBag.FooterInfo = website?.FooterInfo ?? "版权所有";
            
            // 获取文章详情
            var article = await GetArticleById(id, websiteId);
            if (article == null)
            {
                return NotFound();
            }
            
            // 构建ViewModel
            var viewModel = new ArticleViewModel
            {
                Id = article.Id,
                Title = article.Title,
                Subtitle = article.Subtitle,
                Content = article.Content,
                CoverImage = article.CoverImage,
                VideoUrl = article.VideoUrl,
                Author = article.Author,
                Source = article.Source,
                PublishTime = article.PublishTime,
                ViewCount = article.ViewCount,
                Summary = article.Summary,
                ChannelName = article.ChannelName,
                ChannelSlug = channelSlug,
                Tags = article.Tags,
                PrevArticle = await GetPrevArticle(id, websiteId),
                NextArticle = await GetNextArticle(id, websiteId),
                RelatedArticles = await GetRelatedArticles(id, websiteId),
                SeoTitle = article.SeoTitle ?? article.Title,
                SeoDescription = article.SeoDescription ?? article.Summary,
                SeoKeywords = article.SeoKeywords ?? string.Join(",", article.Tags),
                CanonicalUrl = $"/{channelSlug}/{year}/{month}/{day}/{id}.html"
            };
            
            // 设置SEO信息
            ViewData["SeoTitle"] = viewModel.SeoTitle;
            ViewData["SeoDescription"] = viewModel.SeoDescription;
            ViewData["SeoKeywords"] = viewModel.SeoKeywords;
            ViewData["CanonicalUrl"] = viewModel.CanonicalUrl;
            
            return View(viewModel);
        }

        /// <summary>
        /// 根据ID获取文章信息
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <param name="websiteId">网站ID</param>
        /// <returns>文章视图模型</returns>
        private async Task<ArticleViewModel> GetArticleById(int id, int websiteId)
        {
            // 实际实现中，应该调用服务获取文章详情
            // 这里为了演示，返回模拟数据
            return new ArticleViewModel
            {
                Id = id,
                Title = "文章标题：重大事件详细报道",
                Subtitle = "副标题：事件的最新进展",
                Content = "<p>这是文章的详细内容，包含多个段落。</p><p>这里是第二段落，详细描述事件的经过。</p><p>这里是第三段落，分析事件的影响。</p>",
                CoverImage = "/images/article.jpg",
                VideoUrl = "",
                Author = "记者",
                Source = "新闻网",
                PublishTime = System.DateTime.Now.AddHours(-2),
                ViewCount = 5000,
                Summary = "这是文章的摘要，简要介绍文章内容。",
                ChannelName = "国内新闻",
                Tags = new List<string> { "新闻", "事件", "报道" },
                SeoTitle = "文章标题：重大事件详细报道",
                SeoDescription = "这是文章的摘要，简要介绍文章内容。",
                SeoKeywords = "新闻,事件,报道"
            };
        }

        /// <summary>
        /// 获取上一篇文章
        /// </summary>
        /// <param name="id">当前文章ID</param>
        /// <param name="websiteId">网站ID</param>
        /// <returns>上一篇文章信息</returns>
        private async Task<ArticleListItemViewModel> GetPrevArticle(int id, int websiteId)
        {
            // 实际实现中，应该调用服务获取上一篇文章
            // 这里为了演示，返回模拟数据
            return new ArticleListItemViewModel
            {
                Id = id - 1,
                Title = "上一篇文章标题",
                Summary = "上一篇文章摘要",
                CoverImage = "/images/prev.jpg",
                PublishTime = System.DateTime.Now.AddHours(-3),
                ChannelName = "国内新闻",
                ChannelSlug = "news/domestic",
                ViewCount = 4000,
                Author = "记者"
            };
        }

        /// <summary>
        /// 获取下一篇文章
        /// </summary>
        /// <param name="id">当前文章ID</param>
        /// <param name="websiteId">网站ID</param>
        /// <returns>下一篇文章信息</returns>
        private async Task<ArticleListItemViewModel> GetNextArticle(int id, int websiteId)
        {
            // 实际实现中，应该调用服务获取下一篇文章
            // 这里为了演示，返回模拟数据
            return new ArticleListItemViewModel
            {
                Id = id + 1,
                Title = "下一篇文章标题",
                Summary = "下一篇文章摘要",
                CoverImage = "/images/next.jpg",
                PublishTime = System.DateTime.Now.AddHours(-1),
                ChannelName = "国内新闻",
                ChannelSlug = "news/domestic",
                ViewCount = 3000,
                Author = "记者"
            };
        }

        /// <summary>
        /// 获取相关文章
        /// </summary>
        /// <param name="id">当前文章ID</param>
        /// <param name="websiteId">网站ID</param>
        /// <returns>相关文章列表</returns>
        private async Task<List<ArticleListItemViewModel>> GetRelatedArticles(int id, int websiteId)
        {
            // 实际实现中，应该调用服务获取相关文章
            // 这里为了演示，返回模拟数据
            var articles = new List<ArticleListItemViewModel>();
            for (int i = 1; i <= 5; i++)
            {
                articles.Add(new ArticleListItemViewModel
                {
                    Id = id + 10 + i,
                    Title = $"相关文章{i}：相关内容",
                    Summary = $"相关文章{i}的摘要",
                    CoverImage = $"/images/related{i}.jpg",
                    PublishTime = System.DateTime.Now.AddDays(-i),
                    ChannelName = "国内新闻",
                    ChannelSlug = "news/domestic",
                    ViewCount = 2000 + i * 100,
                    Author = "记者"
                });
            }
            return articles;
        }
    }
}