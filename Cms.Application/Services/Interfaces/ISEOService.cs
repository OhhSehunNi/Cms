namespace Cms.Application.Services
{
    /// <summary>
    /// SEO 服务接口，用于 SEO 相关的业务逻辑
    /// </summary>
    public interface ISEOService
    {
        /// <summary>
        /// 生成网站地图
        /// </summary>
        /// <returns>网站地图 XML 字符串</returns>
        Task<string> GenerateSitemapAsync();

        /// <summary>
        /// 生成 robots.txt 文件内容
        /// </summary>
        /// <returns>robots.txt 文本内容</returns>
        Task<string> GenerateRobotsTxtAsync();

        /// <summary>
        /// 生成面包屑导航
        /// </summary>
        /// <param name="channelId">栏目 ID</param>
        /// <param name="articleId">文章 ID</param>
        /// <returns>面包屑导航 HTML 字符串</returns>
        Task<string> GenerateBreadcrumbsAsync(int? channelId = null, int? articleId = null);
    }
}
