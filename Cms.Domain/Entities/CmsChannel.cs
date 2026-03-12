namespace Cms.Domain.Entities
{
    public class CmsChannel : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public int? ParentId { get; set; }
        public CmsChannel Parent { get; set; }
        public List<CmsChannel> Children { get; set; } = new List<CmsChannel>();
        public int SortOrder { get; set; }
        public bool IsShowInNav { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
        public string TemplateType { get; set; }
        public bool IsEnabled { get; set; } = true;
        public List<CmsArticle> Articles { get; set; } = new List<CmsArticle>();
    }
}