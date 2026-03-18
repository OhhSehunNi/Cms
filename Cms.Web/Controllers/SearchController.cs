using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cms.Web.ViewModels;
using Cms.Application.Services;

namespace Cms.Web.Controllers
{
    /// <summary>
    /// 搜索控制器
    /// 处理网站搜索功能，包括关键词搜索和结果展示
    /// </summary>
    public class SearchController : Controller
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
        public SearchController(IArticleService articleService, IWebsiteService websiteService)
        {
            _articleService = articleService;
            _websiteService = websiteService;
        }

        /// <summary>
        /// 搜索结果页
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="page">页码，默认1</param>
        /// <returns>搜索结果视图</returns>
        [Route("search")]
        public async Task<IActionResult> Index(string keyword, int page = 1)
        {
            // 从当前请求上下文中获取WebsiteId
            var websiteId = HttpContext.Items["WebsiteId"] as int? ?? 1;
            
            // 获取网站信息
            var website = await _websiteService.GetByIdAsync(websiteId);
            ViewBag.WebsiteName = website?.Name ?? "新闻网站";
            ViewBag.FooterInfo = website?.FooterInfo ?? "版权所有";
            
            // 获取搜索结果
            var articles = await SearchArticles(keyword, websiteId, page, 20);
            
            // 构建ViewModel
            var viewModel = new SearchViewModel
            {
                Keyword = keyword,
                Articles = articles,
                TotalCount = 50, // 实际应从服务获取
                PageIndex = page,
                PageSize = 20,
                SeoTitle = $"搜索：{keyword} - {website?.Name}",
                SeoDescription = $"关于{keyword}的搜索结果",
                SeoKeywords = keyword
            };
            
            // 设置SEO信息
            ViewData["SeoTitle"] = viewModel.SeoTitle;
            ViewData["SeoDescription"] = viewModel.SeoDescription;
            ViewData["SeoKeywords"] = viewModel.SeoKeywords;
            
            return View(viewModel);
        }

        /// <summary>
        /// 搜索文章
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="websiteId">网站ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>搜索结果文章列表</returns>
        private async Task<List<ArticleListItemViewModel>> SearchArticles(string keyword, int websiteId, int page, int pageSize)
        {
            // 实际实现中，应该调用服务搜索文章
            // 这里为了演示，返回模拟数据
            var articles = new List<ArticleListItemViewModel>();
            for (int i = 1; i <= pageSize; i++)
            {
                articles.Add(new ArticleListItemViewModel
                {
                    Id = (page - 1) * pageSize + i + 400,
                    Title = $"搜索结果{i}：{keyword}相关内容",
                    Summary = $"这是关于{keyword}的搜索结果{i}的摘要",
                    CoverImage = $"/images/search{i}.jpg",
                    PublishTime = System.DateTime.Now.AddHours(-i),
                    ChannelName = "新闻",
                    ChannelSlug = "news",
                    ViewCount = 600 + i * 30,
                    Author = "记者"
                });
            }
            return articles;
        }
    }
}