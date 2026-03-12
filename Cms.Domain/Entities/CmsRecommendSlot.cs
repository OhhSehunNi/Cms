namespace Cms.Domain.Entities
{
    public class CmsRecommendSlot : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public int SortOrder { get; set; }
        public bool IsEnabled { get; set; } = true;
        public List<CmsRecommendItem> RecommendItems { get; set; } = new List<CmsRecommendItem>();
    }
}