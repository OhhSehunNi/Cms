namespace Cms.Domain.Entities
{
    /// <summary>
    /// 角色权限关联实体类，用于管理角色和权限的多对多关系
    /// </summary>
    public class CmsRolePermission : BaseEntity
    {
        /// <summary>
        /// 角色 ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色对象
        /// </summary>
        public CmsRole Role { get; set; }

        /// <summary>
        /// 权限 ID
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// 权限对象
        /// </summary>
        public CmsPermission Permission { get; set; }
    }
}