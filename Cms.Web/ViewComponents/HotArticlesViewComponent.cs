using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cms.Web.ViewModels;
using Cms.Application.Services;
using Cms.Application.Services.Dtos;

namespace Cms.Web.ViewComponents
{
    /// <summary>
    /// 热门文章视图组件
    /// 用于显示热门文章列表
    /// </summary>
    public class HotArticlesViewComponent : ViewComponent
    {
        /// <summary>
        /// 文章服务接口
        /// </summary>
        private readonly IArticleService _articleService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="articleService">文章服务实例</param>
        public HotArticlesViewComponent(IArticleService articleService)
        {
            _articleService = articleService;
        }

        /// <summary>
        /// 调用视图组件
        /// </summary>
        /// <param name="count">显示文章数量，默认10</param>
        /// <returns>视图组件结果</returns>
        public async Task<IViewComponentResult> InvokeAsync(int count = 10)
        {
            // 从当前请求上下文中获取WebsiteId
            var websiteId = HttpContext.Items["WebsiteId"] as int? ?? 1;
            
            // 获取热门文章数据
            var hotArticles = await GetHotArticles(websiteId, count);
            
            return View(hotArticles);
        }

        /// <summary>
        /// 获取热门文章
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <param name="count">文章数量</param>
        /// <returns>热门文章列表</returns>
        private async Task<List<ArticleListItemViewModel>> GetHotArticles(int websiteId, int count)
        {
            // 调用服务获取热门文章，按浏览量排序
            var articles = await _articleService.GetListAsync(1, count, null, null);
            
            // 转换为ViewModel
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
            }).OrderByDescending(a => a.ViewCount).ToList();
        }
    }
}