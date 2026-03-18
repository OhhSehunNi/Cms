namespace Cms.Application.Services.Dtos
{
    /// <summary>
    /// 网站数据传输对象，用于网站相关的请求和响应
    /// </summary>
    public class WebsiteDto
    {
        /// <summary>
        /// 网站 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 网站名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 网站域名
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 网站 logo
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        /// SEO 标题
        /// </summary>
        public string? SeoTitle { get; set; }

        /// <summary>
        /// SEO 描述
        /// </summary>
        public string? SeoDescription { get; set; }

        /// <summary>
        /// SEO 关键词
        /// </summary>
        public string? SeoKeywords { get; set; }

        /// <summary>
        /// 网站底部信息
        /// </summary>
        public string? FooterInfo { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
