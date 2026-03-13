namespace Cms.Application.DTOs
{
    /// <summary>
    /// 专题数据传输对象，用于专题相关的请求和响应
    /// </summary>
    public class TopicDto
    {
        /// <summary>
        /// 专题 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 专题名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 专题 slug（URL 友好的名称）
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// 专题封面图片
        /// </summary>
        public string? CoverImage { get; set; }

        /// <summary>
        /// 专题描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 网站 ID
        /// </summary>
        public int WebsiteId { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

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
        /// 文章数量
        /// </summary>
        public int ArticleCount { get; set; }

        /// <summary>
        /// 文章 ID 列表
        /// </summary>
        public List<int> ArticleIds { get; set; } = new List<int>();
    }
}