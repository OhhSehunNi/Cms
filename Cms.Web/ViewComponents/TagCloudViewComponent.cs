using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cms.Application.Services;
using Cms.Web.ViewModels;

namespace Cms.Web.ViewComponents
{
    /// <summary>
    /// 标签云视图组件
    /// 用于显示标签云
    /// </summary>
    public class TagCloudViewComponent : ViewComponent
    {
        /// <summary>
        /// 标签服务接口
        /// </summary>
        private readonly ITagService _tagService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tagService">标签服务实例</param>
        public TagCloudViewComponent(ITagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// 调用视图组件
        /// </summary>
        /// <param name="count">显示标签数量，默认20</param>
        /// <returns>视图组件结果</returns>
        public async Task<IViewComponentResult> InvokeAsync(int count = 20)
        {
            // 从当前请求上下文中获取WebsiteId
            var websiteId = HttpContext.Items["WebsiteId"] as int? ?? 1;
            
            // 获取标签云数据
            var tags = await GetTags(websiteId, count);
            
            return View(tags);
        }

        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <param name="count">标签数量</param>
        /// <returns>标签列表</returns>
        private async Task<List<TagViewModel>> GetTags(int websiteId, int count)
        {
            // 调用服务获取带文章数量的标签
            var tagsWithCount = await _tagService.GetTagsWithCountAsync(websiteId, count);
            
            // 转换为ViewModel
            return tagsWithCount.Select(t => new TagViewModel
            {
                TagName = t.Name,
                TagSlug = t.Slug,
                Articles = new List<ArticleListItemViewModel>(),
                TotalCount = t.Count
            }).ToList();
        }
    }
}