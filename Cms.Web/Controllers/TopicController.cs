using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cms.Web.ViewModels;
using Cms.Application.Services;

namespace Cms.Web.Controllers
{
    public class TopicController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ITopicService _topicService;
        private readonly IWebsiteService _websiteService;

        public TopicController(IArticleService articleService, ITopicService topicService, IWebsiteService websiteService)
        {
            _articleService = articleService;
            _topicService = topicService;
            _websiteService = websiteService;
        }

        [Route("special/{topicSlug}")]
        [ResponseCache(Duration = 1800, VaryByHeader = "Host")]
        public async Task<IActionResult> Index(string topicSlug, int page = 1)
        {
            // 从当前请求上下文中获取WebsiteId
            var websiteId = HttpContext.Items["WebsiteId"] as int? ?? 1;
            
            // 获取网站信息
            var website = await _websiteService.GetByIdAsync(websiteId);
            ViewBag.WebsiteName = website?.Name ?? "新闻网站";
            ViewBag.FooterInfo = website?.FooterInfo ?? "版权所有";
            
            // 获取专题信息
            var topic = await GetTopicBySlug(topicSlug, websiteId);
            if (topic == null)
            {
                return NotFound();
            }
            
            // 获取文章列表
            var articles = await GetTopicArticles(topic.TopicId, websiteId, page, 20);
            
            // 构建ViewModel
            var viewModel = new TopicViewModel
            {
                TopicId = topic.TopicId,
                TopicName = topic.TopicName,
                TopicSlug = topic.TopicSlug,
                CoverImage = topic.CoverImage,
                Description = topic.Description,
                CreateTime = topic.CreateTime,
                Articles = articles,
                TotalCount = 60, // 实际应从服务获取
                PageIndex = page,
                PageSize = 20,
                SeoTitle = $"{topic.TopicName} - {website?.Name}",
                SeoDescription = topic.Description,
                SeoKeywords = topic.TopicName
            };
            
            // 设置SEO信息
            ViewData["SeoTitle"] = viewModel.SeoTitle;
            ViewData["SeoDescription"] = viewModel.SeoDescription;
            ViewData["SeoKeywords"] = viewModel.SeoKeywords;
            
            return View(viewModel);
        }

        private async Task<TopicViewModel> GetTopicBySlug(string topicSlug, int websiteId)
        {
            // 实际实现中，应该调用服务获取专题
            // 这里为了演示，返回模拟数据
            return new TopicViewModel
            {
                TopicId = 1,
                TopicName = "专题名称：重大事件专题",
                TopicSlug = topicSlug,
                CoverImage = "/images/topic.jpg",
                Description = "这是一个关于重大事件的专题报道，包含多篇相关文章。",
                CreateTime = System.DateTime.Now.AddDays(-7)
            };
        }

        private async Task<List<ArticleListItemViewModel>> GetTopicArticles(int topicId, int websiteId, int page, int pageSize)
        {
            // 实际实现中，应该调用服务获取文章列表
            // 这里为了演示，返回模拟数据
            var articles = new List<ArticleListItemViewModel>();
            for (int i = 1; i <= pageSize; i++)
            {
                articles.Add(new ArticleListItemViewModel
                {
                    Id = (page - 1) * pageSize + i + 300,
                    Title = $"专题文章{i}：相关报道",
                    Summary = $"这是专题文章{i}的摘要",
                    CoverImage = $"/images/topic_article{i}.jpg",
                    PublishTime = System.DateTime.Now.AddDays(-i),
                    ChannelName = "专题",
                    ChannelSlug = "special",
                    ViewCount = 1200 + i * 60,
                    Author = "记者"
                });
            }
            return articles;
        }
    }
}