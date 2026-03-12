namespace Cms.Domain.Entities
{
    public class CmsPermission : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public List<CmsRolePermission> RolePermissions { get; set; } = new List<CmsRolePermission>();
    }
}