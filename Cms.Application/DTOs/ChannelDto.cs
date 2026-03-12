namespace Cms.Application.DTOs
{
    public class ChannelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public int SortOrder { get; set; }
        public bool IsShowInNav { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
        public string TemplateType { get; set; }
        public bool IsEnabled { get; set; }
        public List<ChannelDto> Children { get; set; } = new List<ChannelDto>();
    }
}