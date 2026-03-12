namespace Cms.Domain.Entities
{
    public class CmsMediaAsset : BaseEntity
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
        public long Size { get; set; }
        public string Group { get; set; }
        public string Extension { get; set; }
    }
}