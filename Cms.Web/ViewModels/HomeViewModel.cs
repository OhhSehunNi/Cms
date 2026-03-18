using System.Collections.Generic;

namespace Cms.Web.ViewModels
{
    /// <summary>
    /// 首页视图模型
    /// 用于网站首页的展示
    /// </summary>
    public class HomeViewModel
    {
        /// <summary>
        /// 焦点图文章
        /// </summary>
        public List<ArticleListItemViewModel> FeaturedArticles { get; set; }
        /// <summary>
        /// 频道区块
        /// </summary>
        public List<ChannelSectionViewModel> ChannelSections { get; set; }
        /// <summary>
        /// 热门文章
        /// </summary>
        public List<ArticleListItemViewModel> HotArticles { get; set; }
        /// <summary>
        /// 标签列表
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 网站名称
        /// </summary>
        public string WebsiteName { get; set; }
        /// <summary>
        /// 网站Logo
        /// </summary>
        public string WebsiteLogo { get; set; }
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

    /// <summary>
    /// 频道区块视图模型
    /// 用于首页频道区块的展示
    /// </summary>
    public class ChannelSectionViewModel
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
        /// 文章列表
        /// </summary>
        public List<ArticleListItemViewModel> Articles { get; set; }
    }
}