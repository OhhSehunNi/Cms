namespace Cms.Domain.Entities
{
    /// <summary>
    /// 推荐位项目实体类，用于管理推荐位中的具体推荐内容
    /// </summary>
    public class CmsRecommendItem : BaseEntity
    {
        /// <summary>
        /// 推荐位 ID
        /// </summary>
        public int SlotId { get; set; }

        /// <summary>
        /// 推荐位对象
        /// </summary>
        public CmsRecommendSlot Slot { get; set; }

        /// <summary>
        /// 文章 ID
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// 文章对象
        /// </summary>
        public CmsArticle Article { get; set; }

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
    }
}