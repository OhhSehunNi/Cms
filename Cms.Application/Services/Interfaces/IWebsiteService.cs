using Cms.Application.Services.Dtos;

namespace Cms.Application.Services
{
    /// <summary>
    /// 网站服务接口，用于网站相关的业务逻辑
    /// </summary>
    public interface IWebsiteService
    {
        /// <summary>
        /// 根据 ID 获取网站
        /// </summary>
        /// <param name="id">网站 ID</param>
        /// <returns>网站 DTO</returns>
        Task<WebsiteDto> GetByIdAsync(int id);

        /// <summary>
        /// 根据域名获取网站
        /// </summary>
        /// <param name="domain">网站域名</param>
        /// <returns>网站 DTO</returns>
        Task<WebsiteDto> GetByDomainAsync(string domain);

        /// <summary>
        /// 获取网站列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <returns>网站 DTO 列表</returns>
        Task<List<WebsiteDto>> GetListAsync(int page, int pageSize, string? keyword = null);

        /// <summary>
        /// 创建网站
        /// </summary>
        /// <param name="websiteDto">网站 DTO</param>
        /// <returns>创建后的网站 DTO</returns>
        Task<WebsiteDto> CreateAsync(WebsiteDto websiteDto);

        /// <summary>
        /// 更新网站
        /// </summary>
        /// <param name="websiteDto">网站 DTO</param>
        /// <returns>更新后的网站 DTO</returns>
        Task<WebsiteDto> UpdateAsync(WebsiteDto websiteDto);

        /// <summary>
        /// 删除网站
        /// </summary>
        /// <param name="id">网站 ID</param>
        /// <returns></returns>
        Task DeleteAsync(int id);
    }
}
