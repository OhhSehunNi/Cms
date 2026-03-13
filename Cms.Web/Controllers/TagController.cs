using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cms.Web.ViewModels;
using Cms.Application.Services;

namespace Cms.Web.Controllers
{
    public class TagController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ITagService _tagService;
        private readonly IWebsiteService _websiteService;

        public TagController(IArticleService articleService, ITagService tagService, IWebsiteService websiteService)
        {
            _articleService = articleService;
            _tagService = tagService;
            _websiteService = websiteService;
        }

        [Route("tag/{tagSlug}")]
        [ResponseCache(Duration = 1800, VaryByHeader = "Host")]
        public async Task<IActionResult> Index(string tagSlug, int page = 1)
        {
            // 从当前请求上下文中获取WebsiteId
            var websiteId = HttpContext.Items["WebsiteId"] as int? ?? 1;
            
            // 获取网站信息
            var website = await _websiteService.GetByIdAsync(websiteId);
            ViewBag.WebsiteName = website?.Name ?? "新闻网站";
            ViewBag.FooterInfo = website?.FooterInfo ?? "版权所有";
            
            // 获取标签信息
            var tag = await GetTagBySlug(tagSlug, websiteId);
            if (tag == null)
            {
                return NotFound();
            }
            
            // 获取文章列表
            var articles = await GetTagArticles(tagSlug, websiteId, page, 20);
            
            // 构建ViewModel
            var viewModel = new TagViewModel
            {
                TagName = tag.TagName,
                TagSlug = tag.TagSlug,
                Articles = articles,
                TotalCount = 80, // 实际应从服务获取
                PageIndex = page,
                PageSize = 20,
                SeoTitle = $"{tag.TagName} - {website?.Name}",
                SeoDescription = $"关于{tag.TagName}的相关新闻",
                SeoKeywords = tag.TagName
            };
            
            // 设置SEO信息
            ViewData["SeoTitle"] = viewModel.SeoTitle;
            ViewData["SeoDescription"] = viewModel.SeoDescription;
            ViewData["SeoKeywords"] = viewModel.SeoKeywords;
            
            return View(viewModel);
        }

        private async Task<TagViewModel> GetTagBySlug(string tagSlug, int websiteId)
        {
            // 实际实现中，应该调用服务获取标签
            // 这里为了演示，返回模拟数据
            return new TagViewModel
            {
                TagName = tagSlug,
                TagSlug = tagSlug,
                TotalCount = 80
            };
        }

        private async Task<List<ArticleListItemViewModel>> GetTagArticles(string tagSlug, int websiteId, int page, int pageSize)
        {
            // 实际实现中，应该调用服务获取文章列表
            // 这里为了演示，返回模拟数据
            var articles = new List<ArticleListItemViewModel>();
            for (int i = 1; i <= pageSize; i++)
            {
                articles.Add(new ArticleListItemViewModel
                {
                    Id = (page - 1) * pageSize + i + 200,
                    Title = $"{tagSlug}相关文章{i}",
                    Summary = $"这是{tagSlug}相关文章{i}的摘要",
                    CoverImage = $"/images/tag{i}.jpg",
                    PublishTime = System.DateTime.Now.AddHours(-i),
                    ChannelName = "新闻",
                    ChannelSlug = "news",
                    ViewCount = 800 + i * 40,
                    Author = "记者"
                });
            }
            return articles;
        }
    }
}