using System.Collections.Generic;

namespace Cms.Web.ViewModels
{
    /// <summary>
    /// 频道视图模型
    /// 用于频道列表页的展示
    /// </summary>
    public class ChannelViewModel
    {
        /// <summary>
        /// 频道ID
        /// </summary>
        public int ChannelId { get; set; }
        /// <summary>
        /// 频道名称
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// 频道Slug
        /// </summary>
        public string ChannelSlug { get; set; }
        /// <summary>
        /// 频道描述
        /// </summary>
        public string Description { get; set; }
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