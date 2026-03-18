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
    /// 推荐视图组件
    /// 用于显示推荐文章列表
    /// </summary>
    public class RecommendViewComponent : ViewComponent
    {
        /// <summary>
        /// 推荐服务接口
        /// </summary>
        private readonly IRecommendService _recommendService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="recommendService">推荐服务实例</param>
        public RecommendViewComponent(IRecommendService recommendService)
        {
            _recommendService = recommendService;
        }

        /// <summary>
        /// 调用视图组件
        /// </summary>
        /// <param name="position">推荐位置</param>
        /// <param name="count">显示文章数量，默认5</param>
        /// <returns>视图组件结果</returns>
        public async Task<IViewComponentResult> InvokeAsync(string position, int count = 5)
        {
            // 从当前请求上下文中获取WebsiteId
            var websiteId = HttpContext.Items["WebsiteId"] as int? ?? 1;
            
            // 获取推荐文章数据
            var recommendArticles = await GetRecommendArticles(position, count);
            
            return View(recommendArticles);
        }

        /// <summary>
        /// 获取推荐文章
        /// </summary>
        /// <param name="position">推荐位置</param>
        /// <param name="count">文章数量</param>
        /// <returns>推荐文章列表</returns>
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