namespace Cms.Domain.Entities
{
    public class CmsTag : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public List<CmsArticleTag> ArticleTags { get; set; } = new List<CmsArticleTag>();
    }
}