namespace Cms.Application.DTOs
{
    public class MediaAssetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
        public long Size { get; set; }
        public string Group { get; set; }
        public string Extension { get; set; }
        public string SizeFormatted { get; set; }
    }
}