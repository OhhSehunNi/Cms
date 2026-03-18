using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;

namespace Cms.Application.Services
{
    /// <summary>
    /// SEO 服务实现类，用于 SEO 相关的业务逻辑
    /// </summary>
    public class SEOService : ISEOService
    {
        private readonly CmsDbContext _dbContext;
        private readonly string _baseUrl;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="configuration">配置</param>
        public SEOService(CmsDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _baseUrl = configuration["BaseUrl"] ?? "https://localhost:5000";
        }

        /// <summary>
        /// 生成网站地图
        /// </summary>
        /// <returns>网站地图 XML 字符串</returns>
        public async Task<string> GenerateSitemapAsync()
        {
            var articles = await _dbContext.CmsArticles
                .Include(a => a.Channel)
                .Where(a => a.Status == "Published" && !a.IsDeleted)
                .OrderByDescending(a => a.PublishTime)
                .ToListAsync();

            var channels = await _dbContext.CmsChannels
                .Where(c => c.IsEnabled && !c.IsDeleted)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            var urlElements = articles.Select(a => new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "url",
                new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "loc", $"{_baseUrl}/article/{a.Id}"),
                new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "lastmod", a.UpdatedAt.ToString("yyyy-MM-dd")),
                new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "changefreq", "daily"),
                new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "priority", "0.8")
            )).Concat(channels.Select(c => new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "url",
                new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "loc", $"{_baseUrl}/channel/{c.Id}"),
                new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "lastmod", c.UpdatedAt.ToString("yyyy-MM-dd")),
                new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "changefreq", "weekly"),
                new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "priority", "0.6")
            ))).ToArray();

            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "urlset",
                    urlElements
                )
            );

            return sitemap.ToString();
        }

        /// <summary>
        /// 生成 robots.txt 文件内容
        /// </summary>
        /// <returns>robots.txt 文本内容</returns>
        public async Task<string> GenerateRobotsTxtAsync()
        {
            var robotsTxt = $"User-agent: *\n" +
                          $"Allow: /\n" +
                          $"Disallow: /admin/\n" +
                          $"Sitemap: {_baseUrl}/sitemap.xml\n";

            return robotsTxt;
        }

        /// <summary>
        /// 生成面包屑导航
        /// </summary>
        /// <param name="channelId">栏目 ID</param>
        /// <param name="articleId">文章 ID</param>
        /// <returns>面包屑导航 HTML 字符串</returns>
        public async Task<string> GenerateBreadcrumbsAsync(int? channelId = null, int? articleId = null)
        {
            var breadcrumbs = new List<BreadcrumbItem>();

            breadcrumbs.Add(new BreadcrumbItem
            {
                Title = "首页",
                Url = "/"
            });

            if (channelId.HasValue)
            {
                var channel = await _dbContext.CmsChannels.FindAsync(channelId.Value);
                if (channel != null)
                {
                    var channelPath = await GetChannelPath(channel);
                    breadcrumbs.AddRange(channelPath);
                }
            }

            if (articleId.HasValue)
            {
                var article = await _dbContext.CmsArticles
                    .Include(a => a.Channel)
                    .FirstOrDefaultAsync(a => a.Id == articleId.Value);

                if (article != null)
                {
                    if (article.Channel != null)
                    {
                        var channelPath = await GetChannelPath(article.Channel);
                        breadcrumbs.AddRange(channelPath);
                    }

                    breadcrumbs.Add(new BreadcrumbItem
                    {
                        Title = article.Title,
                        Url = $"/article/{article.Id}"
                    });
                }
            }

            var html = string.Join(" &gt; ", breadcrumbs.Select(b => $"<a href=\"{b.Url}\">{b.Title}</a>"));
            return html;
        }

        /// <summary>
        /// 获取栏目路径
        /// </summary>
        /// <param name="channel">栏目实体</param>
        /// <returns>面包屑项列表</returns>
        private async Task<List<BreadcrumbItem>> GetChannelPath(CmsChannel channel)
        {
            var path = new List<BreadcrumbItem>();
            var current = channel;

            while (current != null)
            {
                path.Insert(0, new BreadcrumbItem
                {
                    Title = current.Name,
                    Url = $"/channel/{current.Id}"
                });

                if (current.ParentId.HasValue)
                {
                    current = await _dbContext.CmsChannels.FindAsync(current.ParentId.Value);
                }
                else
                {
                    current = null;
                }
            }

            return path;
        }

        /// <summary>
        /// 面包屑项
        /// </summary>
        private class BreadcrumbItem
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// 链接地址
            /// </summary>
            public string Url { get; set; }
        }
    }
}
