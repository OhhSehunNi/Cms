using System.ComponentModel.DataAnnotations.Schema;

namespace Cms.Domain.Entities
{
    /// <summary>
    /// 标签实体类，用于管理文章标签
    /// </summary>
    public class CmsTag : BaseEntity
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 标签 slug（URL 友好的名称）
        /// </summary>
        public string Slug { get; set; }

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
        /// 标签文章关联列表
        /// </summary>
        public List<CmsArticleTag> ArticleTags { get; set; } = new List<CmsArticleTag>();
    }
}