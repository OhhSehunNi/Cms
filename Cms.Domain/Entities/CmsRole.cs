namespace Cms.Domain.Entities
{
    public class CmsRole : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; } = true;
        public List<CmsUserRole> UserRoles { get; set; } = new List<CmsUserRole>();
        public List<CmsRolePermission> RolePermissions { get; set; } = new List<CmsRolePermission>();
    }
}