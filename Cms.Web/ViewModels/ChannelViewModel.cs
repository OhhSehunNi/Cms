using System.Collections.Generic;

namespace Cms.Web.ViewModels
{
    public class ChannelViewModel
    {
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string ChannelSlug { get; set; }
        public string Description { get; set; }
        public List<ArticleListItemViewModel> Articles { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
    }
}