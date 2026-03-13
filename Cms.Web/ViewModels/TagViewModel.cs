using System.Collections.Generic;

namespace Cms.Web.ViewModels
{
    public class TagViewModel
    {
        public string TagName { get; set; }
        public string TagSlug { get; set; }
        public List<ArticleListItemViewModel> Articles { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
    }
}