using System;

namespace Cms.Web.ViewModels
{
    /// <summary>
    /// 文章视图模型
    /// 用于文章详情页的展示
    /// </summary>
    public class ArticleViewModel
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 文章副标题
        /// </summary>
        public string Subtitle { get; set; }
        /// <summary>
        /// 文章内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 封面图片
        /// </summary>
        public string CoverImage { get; set; }
        /// <summary>
        /// 视频链接
        /// </summary>
        public string VideoUrl { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishTime { get; set; }
        /// <summary>
        /// 浏览量
        /// </summary>
        public int ViewCount { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 频道名称
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// 频道Slug
        /// </summary>
        public string ChannelSlug { get; set; }
        /// <summary>
        /// 标签列表
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 上一篇文章
        /// </summary>
        public ArticleListItemViewModel PrevArticle { get; set; }
        /// <summary>
        /// 下一篇文章
        /// </summary>
        public ArticleListItemViewModel NextArticle { get; set; }
        /// <summary>
        /// 相关文章
        /// </summary>
        public List<ArticleListItemViewModel> RelatedArticles { get; set; }
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
        /// <summary>
        /// 规范URL
        /// </summary>
        public string CanonicalUrl { get; set; }
    }

    /// <summary>
    /// 文章列表项视图模型
    /// 用于文章列表的展示
    /// </summary>
    public class ArticleListItemViewModel
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 文章摘要
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 封面图片
        /// </summary>
        public string CoverImage { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishTime { get; set; }
        /// <summary>
        /// 频道名称
        /// </summary>
        public string ChannelName { get; set; }
        /// <summary>
        /// 频道Slug
        /// </summary>
        public string ChannelSlug { get; set; }
        /// <summary>
        /// 浏览量
        /// </summary>
        public int ViewCount { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }
    }
}