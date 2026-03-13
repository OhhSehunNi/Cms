using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cms.Web.ViewModels;
using Cms.Application.Services;

namespace Cms.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IChannelService _channelService;
        private readonly IWebsiteService _websiteService;

        public HomeController(IArticleService articleService, IChannelService channelService, IWebsiteService websiteService)
        {
            _articleService = articleService;
            _channelService = channelService;
            _websiteService = websiteService;
        }

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

        private async Task<List<ChannelSectionViewModel>> GetChannelSections(int websiteId)
        {
            // 调用服务获取栏目列表
            var channels = await _channelService.GetTreeAsync();
            var sections = new List<ChannelSectionViewModel>();
            
            // 遍历每个顶级栏目
            foreach (var channel in channels)
            {
                // 获取每个栏目的最新文章
                var articles = await _articleService.GetListAsync(1, 2, null, channel.Id, websiteId);
                
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

        private async Task<List<string>> GetTags(int websiteId)
        {
            // 调用服务获取标签
            var tagService = HttpContext.RequestServices.GetService<ITagService>();
            var tags = await tagService.GetTagsWithCountAsync(websiteId, 20);
            
            return tags.Select(t => t.Name).ToList();
        }
    }
}