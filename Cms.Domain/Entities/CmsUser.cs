namespace Cms.Domain.Entities
{
    public class CmsUser : BaseEntity
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public bool IsEnabled { get; set; } = true;
        public List<CmsUserRole> UserRoles { get; set; } = new List<CmsUserRole>();
    }
}