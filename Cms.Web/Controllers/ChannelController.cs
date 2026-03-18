using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cms.Web.ViewModels;
using Cms.Application.Services;
using Cms.Application.Services.Dtos;

namespace Cms.Web.Controllers
{
    /// <summary>
    /// 频道控制器
    /// 处理频道相关的视图请求，包括频道列表页的展示
    /// </summary>
    public class ChannelController : Controller
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
        public ChannelController(IArticleService articleService, IChannelService channelService, IWebsiteService websiteService)
        {
            _articleService = articleService;
            _channelService = channelService;
            _websiteService = websiteService;
        }

        /// <summary>
        /// 频道列表页
        /// </summary>
        /// <param name="channelSlug">频道Slug</param>
        /// <param name="page">页码，默认1</param>
        /// <returns>频道列表视图</returns>
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

        /// <summary>
        /// 根据Slug获取频道信息
        /// </summary>
        /// <param name="channelSlug">频道Slug</param>
        /// <param name="websiteId">网站ID</param>
        /// <returns>频道视图模型</returns>
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

        /// <summary>
        /// 递归查找频道
        /// </summary>
        /// <param name="channels">频道列表</param>
        /// <param name="slug">频道Slug</param>
        /// <returns>匹配的频道</returns>
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

        /// <summary>
        /// 获取频道文章列表
        /// </summary>
        /// <param name="channelId">频道ID</param>
        /// <param name="websiteId">网站ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>文章列表</returns>
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