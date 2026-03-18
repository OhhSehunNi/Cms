using System;

namespace Cms.Domain.Entities
{
    /// <summary>
    /// 角色栏目关联实体
    /// </summary>
    public class CmsRoleChannel : BaseEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 栏目ID
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// 角色实体
        /// </summary>
        public CmsRole Role { get; set; }

        /// <summary>
        /// 栏目实体
        /// </summary>
        public CmsChannel Channel { get; set; }
    }
}