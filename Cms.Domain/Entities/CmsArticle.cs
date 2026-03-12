namespace Cms.Domain.Entities
{
    public class CmsArticle : BaseEntity
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Summary { get; set; }
        public string CoverImage { get; set; }
        public string VideoUrl { get; set; }
        public int ChannelId { get; set; }
        public CmsChannel Channel { get; set; }
        public string Author { get; set; }
        public string Source { get; set; }
        public DateTime PublishTime { get; set; }
        public string Status { get; set; }
        public bool IsTop { get; set; }
        public bool IsRecommended { get; set; }
        public bool IsHeadline { get; set; }
        public int SortOrder { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
        public string Slug { get; set; }
        public int ViewCount { get; set; }
        public CmsArticleContent Content { get; set; }
        public List<CmsArticleTag> ArticleTags { get; set; } = new List<CmsArticleTag>();
    }
}