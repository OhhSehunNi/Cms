namespace Cms.Application.Services
{
    public interface ISEOService
    {
        Task<string> GenerateSitemapAsync();
        Task<string> GenerateRobotsTxtAsync();
        Task<string> GenerateBreadcrumbsAsync(int? channelId = null, int? articleId = null);
    }
}