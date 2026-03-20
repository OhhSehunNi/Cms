namespace Cms.Domain.Entities
{
    /// <summary>
    /// 文章内容实体类，用于存储文章的详细内容
    /// </summary>
    public class CmsArticleContent : BaseEntity
    {
        /// <summary>
        /// 文章 ID
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// 文章对象
        /// </summary>
        public CmsArticle Article { get; set; }

        /// <summary>
        /// HTML 内容
        /// </summary>
        public string? HtmlContent { get; set; }

        /// <summary>
        /// 纯文本内容
        /// </summary>
        public string? TextContent { get; set; }

        /// <summary>
        /// 字数统计
        /// </summary>
        public int WordCount { get; set; }

        /// <summary>
        /// 扩展 JSON 数据
        /// </summary>
        public string? ExtendJson { get; set; }
    }
}