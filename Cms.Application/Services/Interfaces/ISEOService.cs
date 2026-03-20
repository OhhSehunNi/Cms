using Cms.Domain.Entities;

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
        /// <param name="websiteId">网站 ID</param>
        /// <returns>网站地图 XML 字符串</returns>
        Task<string> GenerateSitemapAsync(int websiteId);

        /// <summary>
        /// 生成 robots.txt 文件内容
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>robots.txt 文本内容</returns>
        Task<string> GenerateRobotsTxtAsync(int websiteId);

        /// <summary>
        /// 生成面包屑导航
        /// </summary>
        /// <param name="channelId">栏目 ID</param>
        /// <param name="articleId">文章 ID</param>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>面包屑导航 HTML 字符串</returns>
        Task<string> GenerateBreadcrumbsAsync(int? channelId = null, int? articleId = null, int? websiteId = null);

        /// <summary>
        /// 获取SEO设置
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>SEO设置</returns>
        Task<object> GetSEOSettingsAsync(int websiteId);

        /// <summary>
        /// 更新SEO设置
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <param name="settings">SEO设置</param>
        /// <returns>是否更新成功</returns>
        Task<bool> UpdateSEOSettingsAsync(int websiteId, object settings);

        /// <summary>
        /// 添加重定向规则
        /// </summary>
        /// <param name="redirect">重定向规则</param>
        /// <returns>重定向规则 ID</returns>
        Task<int> AddRedirectAsync(CmsSeoRedirect redirect);

        /// <summary>
        /// 获取重定向规则列表
        /// </summary>
        /// <param name="websiteId">网站 ID</param>
        /// <returns>重定向规则列表</returns>
        Task<List<CmsSeoRedirect>> GetRedirectsAsync(int websiteId);

        /// <summary>
        /// 删除重定向规则
        /// </summary>
        /// <param name="id">重定向规则 ID</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteRedirectAsync(int id);

        /// <summary>
        /// 生成 Slug
        /// </summary>
        /// <param name="title">标题</param>
        /// <returns>Slug 字符串</returns>
        string GenerateSlug(string title);
    }
}
