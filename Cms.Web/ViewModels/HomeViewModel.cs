using System.Collections.Generic;

namespace Cms.Web.ViewModels
{
    public class HomeViewModel
    {
        public List<ArticleListItemViewModel> FeaturedArticles { get; set; }
        public List<ChannelSectionViewModel> ChannelSections { get; set; }
        public List<ArticleListItemViewModel> HotArticles { get; set; }
        public List<string> Tags { get; set; }
        public string WebsiteName { get; set; }
        public string WebsiteLogo { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
    }

    public class ChannelSectionViewModel
    {
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string ChannelSlug { get; set; }
        public List<ArticleListItemViewModel> Articles { get; set; }
    }
}