namespace Cms.Application.DTOs
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Summary { get; set; }
        public string CoverImage { get; set; }
        public string VideoUrl { get; set; }
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
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
        public string HtmlContent { get; set; }
        public string TextContent { get; set; }
        public List<int> TagIds { get; set; } = new List<int>();
        public List<string> TagNames { get; set; } = new List<string>();
    }
}