using System.ComponentModel.DataAnnotations.Schema;

namespace Cms.Domain.Entities
{
    /// <summary>
    /// SEO重定向实体类，用于管理301重定向规则
    /// </summary>
    public class CmsSeoRedirect : BaseEntity
    {
        /// <summary>
        /// 旧URL
        /// </summary>
        public string OldUrl { get; set; }

        /// <summary>
        /// 新URL
        /// </summary>
        public string NewUrl { get; set; }

        /// <summary>
        /// 是否为永久重定向（301）
        /// </summary>
        public bool IsPermanent { get; set; }

        /// <summary>
        /// 网站ID
        /// </summary>
        [Column("WebsiteId")]
        public int WebsiteId { get; set; }

        /// <summary>
        /// 网站对象
        /// </summary>
        public CmsWebsite Website { get; set; }

        /// <summary>
        /// 创建重定向规则
        /// </summary>
        public void Create()
        {
            IsPermanent = true;
            IsDeleted = false;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 更新重定向规则
        /// </summary>
        public void Update()
        {
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// 删除重定向规则
        /// </summary>
        public void Delete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.Now;
        }
    }
}