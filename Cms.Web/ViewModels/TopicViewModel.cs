using System;using System.Collections.Generic;

namespace Cms.Web.ViewModels
{
    /// <summary>
    /// 专题视图模型
    /// 用于专题列表页的展示
    /// </summary>
    public class TopicViewModel
    {
        /// <summary>
        /// 专题ID
        /// </summary>
        public int TopicId { get; set; }
        /// <summary>
        /// 专题名称
        /// </summary>
        public string TopicName { get; set; }
        /// <summary>
        /// 专题Slug
        /// </summary>
        public string TopicSlug { get; set; }
        /// <summary>
        /// 封面图片
        /// </summary>
        public string CoverImage { get; set; }
        /// <summary>
        /// 专题描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
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