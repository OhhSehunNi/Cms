using System;

namespace Cms.Web.ViewModels
{
    public class ArticleViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Content { get; set; }
        public string CoverImage { get; set; }
        public string VideoUrl { get; set; }
        public string Author { get; set; }
        public string Source { get; set; }
        public DateTime PublishTime { get; set; }
        public int ViewCount { get; set; }
        public string Summary { get; set; }
        public string ChannelName { get; set; }
        public string ChannelSlug { get; set; }
        public List<string> Tags { get; set; }
        public ArticleListItemViewModel PrevArticle { get; set; }
        public ArticleListItemViewModel NextArticle { get; set; }
        public List<ArticleListItemViewModel> RelatedArticles { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
        public string CanonicalUrl { get; set; }
    }

    public class ArticleListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string CoverImage { get; set; }
        public DateTime PublishTime { get; set; }
        public string ChannelName { get; set; }
        public string ChannelSlug { get; set; }
        public int ViewCount { get; set; }
        public string Author { get; set; }
    }
}