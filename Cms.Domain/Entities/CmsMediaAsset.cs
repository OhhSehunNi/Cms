using System.ComponentModel.DataAnnotations.Schema;

namespace Cms.Domain.Entities
{
    /// <summary>
    /// 媒体资源实体类，用于管理网站的图片、视频等媒体文件
    /// </summary>
    public class CmsMediaAsset : BaseEntity
    {
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
        [Column("GroupName")]
        public string Group { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// 网站 ID
        /// </summary>
        [Column("WebsiteId")]
        public int WebsiteId { get; set; }

        /// <summary>
        /// 网站对象
        /// </summary>
        public CmsWebsite Website { get; set; }
    }
}