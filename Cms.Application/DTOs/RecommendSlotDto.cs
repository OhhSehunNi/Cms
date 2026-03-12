namespace Cms.Application.DTOs
{
    public class RecommendSlotDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public int SortOrder { get; set; }
        public bool IsEnabled { get; set; }
        public List<RecommendItemDto> RecommendItems { get; set; } = new List<RecommendItemDto>();
    }

    public class RecommendItemDto
    {
        public int Id { get; set; }
        public int SlotId { get; set; }
        public int ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleCoverImage { get; set; }
        public int SortOrder { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}