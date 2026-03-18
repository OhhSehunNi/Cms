namespace Cms.Application.Services.Dtos
{
    /// <summary>
    /// 角色数据传输对象，用于角色相关的请求和响应
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// 角色 ID
        /// </summary>
        public int Id { get; set; }

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
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 权限列表
        /// </summary>
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }

    /// <summary>
    /// 权限数据传输对象，用于权限相关的请求和响应
    /// </summary>
    public class PermissionDto
    {
        /// <summary>
        /// 权限 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 权限代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Description { get; set; }
    }
}