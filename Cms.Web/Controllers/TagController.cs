using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cms.Web.ViewModels;
using Cms.Application.Services;

namespace Cms.Web.Controllers
{
    /// <summary>
    /// 标签控制器
    /// 处理标签相关的视图请求，包括标签列表页的展示
    /// </summary>
    public class TagController : Controller
    {
        /// <summary>
        /// 文章服务接口
        /// </summary>
        private readonly IArticleService _articleService;
        /// <summary>
        /// 标签服务接口
        /// </summary>
        private readonly ITagService _tagService;
        /// <summary>
        /// 网站服务接口
        /// </summary>
        private readonly IWebsiteService _websiteService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="articleService">文章服务实例</param>
        /// <param name="tagService">标签服务实例</param>
        /// <param name="websiteService">网站服务实例</param>
        public TagController(IArticleService articleService, ITagService tagService, IWebsiteService websiteService)
        {
            _articleService = articleService;
            _tagService = tagService;
            _websiteService = websiteService;
        }

        /// <summary>
        /// 标签列表页
        /// </summary>
        /// <param name="tagSlug">标签Slug</param>
        /// <param name="page">页码，默认1</param>
        /// <returns>标签列表视图</returns>
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

        /// <summary>
        /// 根据Slug获取标签信息
        /// </summary>
        /// <param name="tagSlug">标签Slug</param>
        /// <param name="websiteId">网站ID</param>
        /// <returns>标签视图模型</returns>
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

        /// <summary>
        /// 获取标签相关文章列表
        /// </summary>
        /// <param name="tagSlug">标签Slug</param>
        /// <param name="websiteId">网站ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>文章列表</returns>
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