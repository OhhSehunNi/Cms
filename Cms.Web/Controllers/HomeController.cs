using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cms.Web.ViewModels;
using Cms.Application.Services;

namespace Cms.Web.Controllers
{
    /// <summary>
    /// 首页控制器
    /// 处理网站首页的视图请求，包括首页内容的展示
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// 文章服务接口
        /// </summary>
        private readonly IArticleService _articleService;
        /// <summary>
        /// 频道服务接口
        /// </summary>
        private readonly IChannelService _channelService;
        /// <summary>
        /// 网站服务接口
        /// </summary>
        private readonly IWebsiteService _websiteService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="articleService">文章服务实例</param>
        /// <param name="channelService">频道服务实例</param>
        /// <param name="websiteService">网站服务实例</param>
        public HomeController(IArticleService articleService, IChannelService channelService, IWebsiteService websiteService)
        {
            _articleService = articleService;
            _channelService = channelService;
            _websiteService = websiteService;
        }

        /// <summary>
        /// 网站首页
        /// </summary>
        /// <returns>首页视图</returns>
        [ResponseCache(Duration = 3600, VaryByHeader = "Host")]
        public async Task<IActionResult> Index()
        {
            // 从当前请求上下文中获取WebsiteId
            var websiteId = HttpContext.Items["WebsiteId"] as int? ?? 1;
            
            // 获取网站信息
            var website = await _websiteService.GetByIdAsync(websiteId);
            ViewBag.WebsiteName = website?.Name ?? "新闻网站";
            ViewBag.FooterInfo = website?.FooterInfo ?? "版权所有";
            
            // 构建首页ViewModel
            var viewModel = new HomeViewModel
            {
                FeaturedArticles = await GetFeaturedArticles(websiteId),
                ChannelSections = await GetChannelSections(websiteId),
                HotArticles = await GetHotArticles(websiteId),
                Tags = await GetTags(websiteId),
                WebsiteName = website?.Name ?? "新闻网站",
                WebsiteLogo = website?.Logo ?? "",
                SeoTitle = website?.SeoTitle ?? "新闻网站首页",
                SeoDescription = website?.SeoDescription ?? "提供最新新闻资讯",
                SeoKeywords = website?.SeoKeywords ?? "新闻,资讯"
            };
            
            // 设置SEO信息
            ViewData["SeoTitle"] = viewModel.SeoTitle;
            ViewData["SeoDescription"] = viewModel.SeoDescription;
            ViewData["SeoKeywords"] = viewModel.SeoKeywords;
            
            return View(viewModel);
        }

        /// <summary>
        /// 获取焦点图文章
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>焦点图文章列表</returns>
        private async Task<List<ArticleListItemViewModel>> GetFeaturedArticles(int websiteId)
        {
            // 调用服务获取焦点图文章
            var articles = await _articleService.GetHeadlineArticlesAsync(websiteId, 5);
            
            return articles.Select(a => new ArticleListItemViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Summary = a.Summary,
                CoverImage = a.CoverImage,
                PublishTime = a.PublishTime,
                ChannelName = a.ChannelName,
                ChannelSlug = a.ChannelSlug,
                ViewCount = a.ViewCount,
                Author = a.Author
            }).ToList();
        }

        /// <summary>
        /// 获取频道区块
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>频道区块列表</returns>
        private async Task<List<ChannelSectionViewModel>> GetChannelSections(int websiteId)
        {
            // 调用服务获取栏目列表
            var channels = await _channelService.GetTreeAsync(websiteId);
            var sections = new List<ChannelSectionViewModel>();
            
            // 遍历每个顶级栏目
            foreach (var channel in channels)
            {
                // 获取每个栏目的最新文章
                var articles = await _articleService.GetListAsync(1, 2, null, channel.Id);
                
                sections.Add(new ChannelSectionViewModel
                {
                    ChannelId = channel.Id,
                    ChannelName = channel.Name,
                    ChannelSlug = channel.Slug,
                    Articles = articles.Select(a => new ArticleListItemViewModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Summary = a.Summary,
                        CoverImage = a.CoverImage,
                        PublishTime = a.PublishTime,
                        ChannelName = a.ChannelName,
                        ChannelSlug = a.ChannelSlug,
                        ViewCount = a.ViewCount,
                        Author = a.Author
                    }).ToList()
                });
            }
            
            return sections;
        }

        /// <summary>
        /// 获取热门文章
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>热门文章列表</returns>
        private async Task<List<ArticleListItemViewModel>> GetHotArticles(int websiteId)
        {
            // 调用服务获取热门文章
            var articles = await _articleService.GetHotArticlesAsync(websiteId, 10);
            
            return articles.Select(a => new ArticleListItemViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Summary = a.Summary,
                CoverImage = a.CoverImage,
                PublishTime = a.PublishTime,
                ChannelName = a.ChannelName,
                ChannelSlug = a.ChannelSlug,
                ViewCount = a.ViewCount,
                Author = a.Author
            }).ToList();
        }

        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>标签列表</returns>
        private async Task<List<string>> GetTags(int websiteId)
        {
            // 调用服务获取标签
            var tagService = HttpContext.RequestServices.GetService<ITagService>();
            var tags = await tagService.GetTagsWithCountAsync(websiteId, 20);
            
            return tags.Select(t => t.Name).ToList();
        }
    }
}