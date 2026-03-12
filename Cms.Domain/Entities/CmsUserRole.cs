namespace Cms.Domain.Entities
{
    public class CmsUserRole : BaseEntity
    {
        public int UserId { get; set; }
        public CmsUser User { get; set; }
        public int RoleId { get; set; }
        public CmsRole Role { get; set; }
    }
}