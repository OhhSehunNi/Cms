using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cms.Web.ViewModels;
using Cms.Application.Services;
using Cms.Application.DTOs;

namespace Cms.Web.Controllers
{
    public class ChannelController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IChannelService _channelService;
        private readonly IWebsiteService _websiteService;

        public ChannelController(IArticleService articleService, IChannelService channelService, IWebsiteService websiteService)
        {
            _articleService = articleService;
            _channelService = channelService;
            _websiteService = websiteService;
        }

        [Route("{channelSlug}")]
        [ResponseCache(Duration = 1800, VaryByHeader = "Host")]
        public async Task<IActionResult> Index(string channelSlug, int page = 1)
        {
            // 从当前请求上下文中获取WebsiteId
            var websiteId = HttpContext.Items["WebsiteId"] as int? ?? 1;
            
            // 获取网站信息
            var website = await _websiteService.GetByIdAsync(websiteId);
            ViewBag.WebsiteName = website?.Name ?? "新闻网站";
            ViewBag.FooterInfo = website?.FooterInfo ?? "版权所有";
            
            // 获取栏目信息
            var channel = await GetChannelBySlug(channelSlug, websiteId);
            if (channel == null)
            {
                return NotFound();
            }
            
            // 获取文章列表
            var articles = await GetChannelArticles(channel.ChannelId, websiteId, page, 20);
            
            // 构建ViewModel
            var viewModel = new ChannelViewModel
            {
                ChannelId = channel.ChannelId,
                ChannelName = channel.ChannelName,
                ChannelSlug = channel.ChannelSlug,
                Description = channel.Description,
                Articles = articles,
                TotalCount = 100, // 实际应从服务获取
                PageIndex = page,
                PageSize = 20,
                SeoTitle = channel.SeoTitle ?? $"{channel.ChannelName} - {website?.Name}",
                SeoDescription = channel.SeoDescription ?? channel.Description,
                SeoKeywords = channel.SeoKeywords ?? channel.ChannelName
            };
            
            // 设置SEO信息
            ViewData["SeoTitle"] = viewModel.SeoTitle;
            ViewData["SeoDescription"] = viewModel.SeoDescription;
            ViewData["SeoKeywords"] = viewModel.SeoKeywords;
            
            return View(viewModel);
        }

        private async Task<ChannelViewModel> GetChannelBySlug(string channelSlug, int websiteId)
        {
            // 调用服务获取栏目
            var channels = await _channelService.GetTreeAsync();
            
            // 查找匹配的栏目
            var channel = FindChannelBySlug(channels, channelSlug);
            
            if (channel == null)
            {
                return null;
            }
            
            return new ChannelViewModel
            {
                ChannelId = channel.Id,
                ChannelName = channel.Name,
                ChannelSlug = channel.Slug,
                Description = channel.SeoDescription,
                SeoTitle = channel.SeoTitle,
                SeoDescription = channel.SeoDescription,
                SeoKeywords = channel.SeoKeywords
            };
        }

        private ChannelDto FindChannelBySlug(List<ChannelDto> channels, string slug)
        {
            foreach (var channel in channels)
            {
                if (channel.Slug == slug)
                {
                    return channel;
                }
                if (channel.Children != null && channel.Children.Count > 0)
                {
                    var found = FindChannelBySlug(channel.Children, slug);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null;
        }

        private async Task<List<ArticleListItemViewModel>> GetChannelArticles(int channelId, int websiteId, int page, int pageSize)
        {
            // 调用服务获取文章列表
            var articles = await _articleService.GetListAsync(page, pageSize, null, channelId, websiteId);
            
            // 转换为ViewModel
            return articles.Select(a => new ArticleListItemViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Summary = a.Summary,
                CoverImage = a.CoverImage,
                PublishTime = a.PublishTime,
                ChannelName = a.ChannelName,
                ChannelSlug = a.Slug,
                ViewCount = a.ViewCount,
                Author = a.Author
            }).ToList();
        }
    }
}