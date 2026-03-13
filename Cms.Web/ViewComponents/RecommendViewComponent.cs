using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cms.Web.ViewModels;
using Cms.Application.Services;
using Cms.Application.DTOs;

namespace Cms.Web.ViewComponents
{
    public class RecommendViewComponent : ViewComponent
    {
        private readonly IRecommendService _recommendService;

        public RecommendViewComponent(IRecommendService recommendService)
        {
            _recommendService = recommendService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string position, int count = 5)
        {
            // 从当前请求上下文中获取WebsiteId
            var websiteId = HttpContext.Items["WebsiteId"] as int? ?? 1;
            
            // 获取推荐文章数据
            var recommendArticles = await GetRecommendArticles(position, count);
            
            return View(recommendArticles);
        }

        private async Task<List<ArticleListItemViewModel>> GetRecommendArticles(string position, int count)
        {
            // 根据位置确定推荐位代码
            string slotCode = position switch
            {
                "sidebar" => "homepage_recommended",
                "carousel" => "homepage_carousel",
                "headline" => "homepage_headline",
                _ => "homepage_recommended"
            };

            // 调用服务获取推荐文章
            var articles = await _recommendService.GetRecommendArticlesAsync(slotCode, count);
            
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
            }).ToList();
        }
    }
}