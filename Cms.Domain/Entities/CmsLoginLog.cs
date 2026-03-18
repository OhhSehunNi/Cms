using System;

namespace Cms.Domain.Entities
{
    /// <summary>
    /// 登录日志实体
    /// </summary>
    public class CmsLoginLog : BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// 关联的用户
        /// </summary>
        public CmsUser User { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 用户代理
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// 登录状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 登录消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}