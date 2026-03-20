using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
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
        /// <param name="websiteId">网站 ID</param>
        /// <returns>网站地图 XML 字符串</returns>
        public async Task<string> GenerateSitemapAsync(int websiteId)
        {
            var articles = await _dbContext.CmsArticles
                .Include(a => a.Channel)
                .Where(a => a.Status == ArticleStatus.Published && !a.IsDeleted && a.WebsiteId == websiteId)
                .OrderByDescending(a => a.PublishTime)
                .ToListAsync();

            var channels = await _dbContext.CmsChannels
                .Where(c => c.IsEnabled && !c.IsDeleted && c.WebsiteId == websiteId)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            var topics = await _dbContext.CmsTopics
                .Where(t => t.IsEnabled && !t.IsDeleted && t.WebsiteId == websiteId)
                .OrderByDescending(t => t.CreatedAt)
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
            ))).Concat(topics.Select(t => new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "url",
                new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "loc", $"{_baseUrl}/topic/{t.Id}"),
                new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "lastmod", t.UpdatedAt.ToString("yyyy-MM-dd")),
                new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "changefreq", "weekly"),
                new XElement(XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9") + "priority", "0.7")
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
        /// <param name="websiteId">网站 ID</param>
        /// <returns>robots.txt 文本内容</returns>
        public async Task<string> GenerateRobotsTxtAsync(int websiteId)
        {
            var website = await _dbContext.CmsWebsites.FindAsync(websiteId);
            var domain = website?.Domain ?? "localhost";
            var baseUrl = _baseUrl.Contains("localhost") ? _baseUrl : $"https://{domain}";

            var robotsTxt = $"User-agent: *\n" +
                          $"Allow: /\n" +
                          $"Disallow: /admin/\n" +
                          $"Sitemap: {baseUrl}/sitemap.xml\n";

            return robotsTxt;
        }

        /// <summary>
        /// 生成面包屑导航
        /// </summary>
        /// <param name="channelId">栏目 ID</param>
        /// <param name="articleId">文章 ID</param>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>面包屑导航 HTML 字符串</returns>
        public async Task<string> GenerateBreadcrumbsAsync(int? channelId = null, int? articleId = null, int? websiteId = null)
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
        /// 获取SEO设置
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>SEO设置</returns>
        public async Task<object> GetSEOSettingsAsync(int websiteId)
        {
            var website = await _dbContext.CmsWebsites.FindAsync(websiteId);
            if (website == null)
            {
                return null;
            }

            return new
            {
                SeoTitle = website.SeoTitle,
                SeoDescription = website.SeoDescription,
                SeoKeywords = website.SeoKeywords
            };
        }

        /// <summary>
        /// 更新SEO设置
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <param name="settings">SEO设置</param>
        /// <returns>是否更新成功</returns>
        public async Task<bool> UpdateSEOSettingsAsync(int websiteId, object settings)
        {
            var website = await _dbContext.CmsWebsites.FindAsync(websiteId);
            if (website == null)
            {
                return false;
            }

            // 动态更新设置
            var settingsDict = settings as System.Collections.Generic.Dictionary<string, object>;
            if (settingsDict != null)
            {
                if (settingsDict.ContainsKey("SeoTitle"))
                    website.SeoTitle = settingsDict["SeoTitle"] as string;
                if (settingsDict.ContainsKey("SeoDescription"))
                    website.SeoDescription = settingsDict["SeoDescription"] as string;
                if (settingsDict.ContainsKey("SeoKeywords"))
                    website.SeoKeywords = settingsDict["SeoKeywords"] as string;

                website.UpdatedAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 添加重定向规则
        /// </summary>
        /// <param name="redirect">重定向规则</param>
        /// <returns>重定向规则 ID</returns>
        public async Task<int> AddRedirectAsync(CmsSeoRedirect redirect)
        {
            redirect.Create();
            _dbContext.CmsSeoRedirects.Add(redirect);
            await _dbContext.SaveChangesAsync();
            return redirect.Id;
        }

        /// <summary>
        /// 获取重定向规则列表
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>重定向规则列表</returns>
        public async Task<List<CmsSeoRedirect>> GetRedirectsAsync(int websiteId)
        {
            return await _dbContext.CmsSeoRedirects
                .Where(r => r.WebsiteId == websiteId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 删除重定向规则
        /// </summary>
        /// <param name="id">重定向规则 ID</param>
        /// <returns>是否删除成功</returns>
        public async Task<bool> DeleteRedirectAsync(int id)
        {
            var redirect = await _dbContext.CmsSeoRedirects.FindAsync(id);
            if (redirect == null)
            {
                return false;
            }

            redirect.Delete();
            await _dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 生成 Slug
        /// </summary>
        /// <param name="title">标题</param>
        /// <returns>Slug 字符串</returns>
        public string GenerateSlug(string title)
        {
            // 移除特殊字符
            var slug = Regex.Replace(title, @"[^a-zA-Z0-9-]", "");
            // 替换空格为连字符
            slug = Regex.Replace(slug, @"\s+", "-");
            // 转换为小写
            slug = slug.ToLower();
            // 移除首尾连字符
            slug = slug.Trim('-');
            // 限制长度
            if (slug.Length > 100)
            {
                slug = slug.Substring(0, 100);
            }
            return slug;
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
