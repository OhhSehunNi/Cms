namespace Cms.Application.Services.Dtos
{
    /// <summary>
    /// 媒体资源数据传输对象，用于媒体资源相关的请求和响应
    /// </summary>
    public class MediaAssetDto
    {
        /// <summary>
        /// 资源 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 资源类型（图片、视频、音频等）
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 资源存储路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 资源访问 URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 资源大小（字节）
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 资源分组
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// 格式化后的文件大小
        /// </summary>
        public string SizeFormatted { get; set; }
    }
}