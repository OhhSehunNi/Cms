namespace Cms.Domain.Entities
{
    public class CmsArticleTag : BaseEntity
    {
        public int ArticleId { get; set; }
        public CmsArticle Article { get; set; }
        public int TagId { get; set; }
        public CmsTag Tag { get; set; }
    }
}