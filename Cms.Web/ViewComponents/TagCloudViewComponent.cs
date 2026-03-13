using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cms.Application.Services;
using Cms.Web.ViewModels;

namespace Cms.Web.ViewComponents
{
    public class TagCloudViewComponent : ViewComponent
    {
        private readonly ITagService _tagService;

        public TagCloudViewComponent(ITagService tagService)
        {
            _tagService = tagService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count = 20)
        {
            // 从当前请求上下文中获取WebsiteId
            var websiteId = HttpContext.Items["WebsiteId"] as int? ?? 1;
            
            // 获取标签云数据
            var tags = await GetTags(websiteId, count);
            
            return View(tags);
        }

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