namespace Cms.Domain.Entities
{
    /// <summary>
    /// 权限实体类，用于管理系统权限
    /// </summary>
    public class CmsPermission : BaseEntity
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 权限角色关联列表
        /// </summary>
        public List<CmsRolePermission> RolePermissions { get; set; } = new List<CmsRolePermission>();
    }
}