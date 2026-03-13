namespace Cms.Domain.Entities
{
    /// <summary>
    /// 用户实体类，用于管理系统用户
    /// </summary>
    public class CmsUser : BaseEntity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码哈希值
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 用户角色关联列表
        /// </summary>
        public List<CmsUserRole> UserRoles { get; set; } = new List<CmsUserRole>();
    }
}