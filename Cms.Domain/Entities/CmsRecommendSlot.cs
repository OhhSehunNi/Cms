using System.ComponentModel.DataAnnotations.Schema;

namespace Cms.Domain.Entities
{
    /// <summary>
    /// 推荐位实体类，用于管理网站的推荐位
    /// </summary>
    public class CmsRecommendSlot : BaseEntity
    {
        /// <summary>
        /// 推荐位名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 推荐位代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 推荐位类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; }

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
        public CmsWebsite Website { get; set; }

        /// <summary>
        /// 推荐位项目列表
        /// </summary>
        public List<CmsRecommendItem> RecommendItems { get; set; } = new List<CmsRecommendItem>();
    }
}