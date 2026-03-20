using System.ComponentModel.DataAnnotations.Schema;

namespace Cms.Domain.Entities
{
    /// <summary>
    /// 文章状态枚举
    /// </summary>
    public static class ArticleStatus
    {
        public const string Draft = "Draft";
        public const string Published = "Published";
        public const string Offline = "Offline";
    }

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

        /// <summary>
        /// 创建文章
        /// </summary>
        public void Create()
        {
            Status = ArticleStatus.Draft;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 更新文章
        /// </summary>
        public void Update()
        {
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 发布文章
        /// </summary>
        public void Publish()
        {
            Status = ArticleStatus.Published;
            PublishTime = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 下线文章
        /// </summary>
        public void Offline()
        {
            Status = ArticleStatus.Offline;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        public void Delete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 检查是否可以发布
        /// </summary>
        /// <returns>是否可以发布</returns>
        public bool CanPublish()
        {
            // 检查是否有正文
            return !string.IsNullOrEmpty(Content?.HtmlContent);
        }

        /// <summary>
        /// 检查是否可以在前台显示
        /// </summary>
        /// <returns>是否可以显示</returns>
        public bool CanShow()
        {
            return Status == ArticleStatus.Published && !IsDeleted;
        }
    }
}