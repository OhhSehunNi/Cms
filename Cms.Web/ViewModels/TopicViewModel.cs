using System;using System.Collections.Generic;

namespace Cms.Web.ViewModels
{
    public class TopicViewModel
    {
        public int TopicId { get; set; }
        public string TopicName { get; set; }
        public string TopicSlug { get; set; }
        public string CoverImage { get; set; }
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
        public List<ArticleListItemViewModel> Articles { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
    }
}