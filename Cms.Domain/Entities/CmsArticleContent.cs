namespace Cms.Domain.Entities
{
    public class CmsArticleContent : BaseEntity
    {
        public int ArticleId { get; set; }
        public CmsArticle Article { get; set; }
        public string HtmlContent { get; set; }
        public string TextContent { get; set; }
        public string ExtendJson { get; set; }
    }
}