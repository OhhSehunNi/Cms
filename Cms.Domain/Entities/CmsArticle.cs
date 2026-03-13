using System.ComponentModel.DataAnnotations.Schema;

namespace Cms.Domain.Entities
{
    /// <summary>
    /// 文章实体类，用于管理网站文章
    /// </summary>
    public class CmsArticle : BaseEntity
    {
        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 文章副标题
        /// </summary>
        public string? SubTitle { get; set; }

        /// <summary>
        /// 文章摘要
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// 文章封面图片
        /// </summary>
        public string? CoverImage { get; set; }

        /// <summary>
        /// 文章视频 URL
        /// </summary>
        public string? VideoUrl { get; set; }

        /// <summary>
        /// 栏目 ID
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// 栏目对象
        /// </summary>
        public CmsChannel Channel { get; set; }

        /// <summary>
        /// 文章作者
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// 文章来源
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishTime { get; set; }

        /// <summary>
        /// 文章状态（草稿、已发布、已下线等）
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }

        /// <summary>
        /// 是否推荐
        /// </summary>
        public bool IsRecommended { get; set; }

        /// <summary>
        /// 是否头条
        /// </summary>
        public bool IsHeadline { get; set; }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; }

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
        /// 文章 slug（URL 友好的名称）
        /// </summary>
        public string? Slug { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 网站 ID
        /// </summary>
        [Column("WebsiteId")]
        public int WebsiteId { get; set; }

        /// <summary>
        /// 网站对象
        /// </summary>
        public CmsWebsite Website { get; set; }

        /// <summary>
        /// 文章内容
        /// </summary>
        public CmsArticleContent? Content { get; set; }

        /// <summary>
        /// 文章标签关联列表
        /// </summary>
        public List<CmsArticleTag> ArticleTags { get; set; } = new List<CmsArticleTag>();
    }
}