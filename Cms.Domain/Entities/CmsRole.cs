namespace Cms.Domain.Entities
{
    /// <summary>
    /// 角色实体类，用于管理系统角色
    /// </summary>
    public class CmsRole : BaseEntity
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 角色用户关联列表
        /// </summary>
        public List<CmsUserRole> UserRoles { get; set; } = new List<CmsUserRole>();

        /// <summary>
        /// 角色权限关联列表
        /// </summary>
        public List<CmsRolePermission> RolePermissions { get; set; } = new List<CmsRolePermission>();
    }
}