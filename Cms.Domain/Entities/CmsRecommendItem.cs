namespace Cms.Domain.Entities
{
    public class CmsRecommendItem : BaseEntity
    {
        public int SlotId { get; set; }
        public CmsRecommendSlot Slot { get; set; }
        public int ArticleId { get; set; }
        public CmsArticle Article { get; set; }
        public int SortOrder { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}