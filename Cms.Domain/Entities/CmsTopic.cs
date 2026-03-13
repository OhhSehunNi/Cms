using System.ComponentModel.DataAnnotations.Schema;

namespace Cms.Domain.Entities
{
    /// <summary>
    /// 专题实体类，用于管理网站专题
    /// </summary>
    public class CmsTopic : BaseEntity
    {
        /// <summary>
        /// 专题名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 专题 slug（URL 友好的名称）
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// 专题封面图片
        /// </summary>
        public string? CoverImage { get; set; }

        /// <summary>
        /// 专题描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 网站 ID
        /// </summary>
        public int WebsiteId { get; set; }

        /// <summary>
        /// 网站对象
        /// </summary>
        public CmsWebsite Website { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// SEO 标题
        /// </summary>
        public string? SeoTitle { get; set; }

        /// <summary>
        /// SEO 描述
        /// </summary>
        public string? SeoDescription { get; set; }

        /// <summary>
        /// SEO 关键词
        /// </summary>
        public string? SeoKeywords { get; set; }

        /// <summary>
        /// 专题文章关联列表
        /// </summary>
        public List<CmsTopicArticle> TopicArticles { get; set; } = new List<CmsTopicArticle>();
    }

    /// <summary>
    /// 专题文章关联实体类，用于管理专题和文章的多对多关系
    /// </summary>
    public class CmsTopicArticle : BaseEntity
    {
        /// <summary>
        /// 专题 ID
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        /// 文章 ID
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 专题对象
        /// </summary>
        public CmsTopic Topic { get; set; }

        /// <summary>
        /// 文章对象
        /// </summary>
        public CmsArticle Article { get; set; }
    }
}