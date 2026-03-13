using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cms.Web.ViewModels;
using Cms.Application.Services;
using Cms.Application.DTOs;

namespace Cms.Web.ViewComponents
{
    public class HotArticlesViewComponent : ViewComponent
    {
        private readonly IArticleService _articleService;

        public HotArticlesViewComponent(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count = 10)
        {
            // 从当前请求上下文中获取WebsiteId
            var websiteId = HttpContext.Items["WebsiteId"] as int? ?? 1;
            
            // 获取热门文章数据
            var hotArticles = await GetHotArticles(websiteId, count);
            
            return View(hotArticles);
        }

        private async Task<List<ArticleListItemViewModel>> GetHotArticles(int websiteId, int count)
        {
            // 调用服务获取热门文章，按浏览量排序
            var articles = await _articleService.GetListAsync(1, count, null, null, websiteId);
            
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