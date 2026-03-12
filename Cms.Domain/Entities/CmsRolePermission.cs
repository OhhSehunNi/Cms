namespace Cms.Domain.Entities
{
    public class CmsRolePermission : BaseEntity
    {
        public int RoleId { get; set; }
        public CmsRole Role { get; set; }
        public int PermissionId { get; set; }
        public CmsPermission Permission { get; set; }
    }
}