using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cms.Domain.Entities
{
    /// <summary>
    /// 栏目实体类，用于管理网站栏目
    /// </summary>
    public class CmsChannel : BaseEntity
    {
        /// <summary>
        /// 栏目名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 栏目 slug（URL 友好的名称）
        /// </summary>
        public string? Slug { get; set; }

        /// <summary>
        /// 父栏目 ID
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 父栏目对象
        /// </summary>
        [ForeignKey("ParentId")]
        public CmsChannel Parent { get; set; }

        /// <summary>
        /// 子栏目列表
        /// </summary>
        public List<CmsChannel> Children { get; set; } = new List<CmsChannel>();

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 是否在导航中显示
        /// </summary>
        public bool IsShowInNav { get; set; }

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
        /// 模板类型
        /// </summary>
        public string? TemplateType { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 网站 ID
        /// </summary>
        [Column("WebsiteId")]
        public int WebsiteId { get; set; }

        /// <summary>
        /// 网站对象
        /// </summary>
        [ForeignKey("WebsiteId")]
        public CmsWebsite Website { get; set; }

        /// <summary>
        /// 栏目下的文章列表
        /// </summary>
        public List<CmsArticle> Articles { get; set; } = new List<CmsArticle>();
    }
}