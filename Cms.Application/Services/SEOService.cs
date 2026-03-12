using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;

namespace Cms.Application.Services
{
    public class SEOService : ISEOService
    {
        private readonly CmsDbContext _dbContext;
        private readonly string _baseUrl;

        public SEOService(CmsDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _baseUrl = configuration["BaseUrl"] ?? "https://localhost:5000";
        }

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

        public async Task<string> GenerateRobotsTxtAsync()
        {
            var robotsTxt = $"User-agent: *\n" +
                          $"Allow: /\n" +
                          $"Disallow: /admin/\n" +
                          $"Sitemap: {_baseUrl}/sitemap.xml\n";

            return robotsTxt;
        }

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

        private class BreadcrumbItem
        {
            public string Title { get; set; }
            public string Url { get; set; }
        }
    }
}