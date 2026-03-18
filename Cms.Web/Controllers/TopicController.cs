using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cms.Web.ViewModels;
using Cms.Application.Services;

namespace Cms.Web.Controllers
{
    /// <summary>
    /// 专题控制器
    /// 处理专题相关的视图请求，包括专题列表页的展示
    /// </summary>
    public class TopicController : Controller
    {
        /// <summary>
        /// 文章服务接口
        /// </summary>
        private readonly IArticleService _articleService;
        /// <summary>
        /// 专题服务接口
        /// </summary>
        private readonly ITopicService _topicService;
        /// <summary>
        /// 网站服务接口
        /// </summary>
        private readonly IWebsiteService _websiteService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="articleService">文章服务实例</param>
        /// <param name="topicService">专题服务实例</param>
        /// <param name="websiteService">网站服务实例</param>
        public TopicController(IArticleService articleService, ITopicService topicService, IWebsiteService websiteService)
        {
            _articleService = articleService;
            _topicService = topicService;
            _websiteService = websiteService;
        }

        /// <summary>
        /// 专题列表页
        /// </summary>
        /// <param name="topicSlug">专题Slug</param>
        /// <param name="page">页码，默认1</param>
        /// <returns>专题列表视图</returns>
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

        /// <summary>
        /// 根据Slug获取专题信息
        /// </summary>
        /// <param name="topicSlug">专题Slug</param>
        /// <param name="websiteId">网站ID</param>
        /// <returns>专题视图模型</returns>
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

        /// <summary>
        /// 获取专题文章列表
        /// </summary>
        /// <param name="topicId">专题ID</param>
        /// <param name="websiteId">网站ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>文章列表</returns>
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