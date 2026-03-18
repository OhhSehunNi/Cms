namespace Cms.Domain.Entities
{
    /// <summary>
    /// 用户角色关联实体类，用于管理用户和角色的多对多关系
    /// </summary>
    public class CmsUserRole : BaseEntity
    {
        /// <summary>
        /// 用户 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户对象
        /// </summary>
        public CmsUser User { get; set; }

        /// <summary>
        /// 角色 ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色对象
        /// </summary>
        public CmsRole Role { get; set; }
    }
}