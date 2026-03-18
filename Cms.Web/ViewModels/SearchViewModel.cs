using System.Collections.Generic;

namespace Cms.Web.ViewModels
{
    /// <summary>
    /// 搜索视图模型
    /// 用于搜索结果页的展示
    /// </summary>
    public class SearchViewModel
    {
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 搜索结果文章列表
        /// </summary>
        public List<ArticleListItemViewModel> Articles { get; set; }
        /// <summary>
        /// 总结果数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// SEO标题
        /// </summary>
        public string SeoTitle { get; set; }
        /// <summary>
        /// SEO描述
        /// </summary>
        public string SeoDescription { get; set; }
        /// <summary>
        /// SEO关键词
        /// </summary>
        public string SeoKeywords { get; set; }
    }
}