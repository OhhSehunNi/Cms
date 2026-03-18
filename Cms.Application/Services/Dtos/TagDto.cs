namespace Cms.Application.Services.Dtos
{
    /// <summary>
    /// 标签数据传输对象，用于标签相关的请求和响应
    /// </summary>
    public class TagDto
    {
        /// <summary>
        /// 标签 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 标签 slug（URL 友好的名称）
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// 网站 ID
        /// </summary>
        public int WebsiteId { get; set; }

        /// <summary>
        /// 文章数量
        /// </summary>
        public int ArticleCount { get; set; }
    }
}