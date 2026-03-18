using System.Collections.Generic;

namespace Cms.Web.ViewModels
{
    /// <summary>
    /// 标签视图模型
    /// 用于标签列表页的展示
    /// </summary>
    public class TagViewModel
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// 标签Slug
        /// </summary>
        public string TagSlug { get; set; }
        /// <summary>
        /// 文章列表
        /// </summary>
        public List<ArticleListItemViewModel> Articles { get; set; }
        /// <summary>
        /// 总文章数
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