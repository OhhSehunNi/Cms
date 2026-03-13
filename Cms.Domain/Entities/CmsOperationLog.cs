namespace Cms.Domain.Entities
{
    /// <summary>
    /// 操作日志实体类，用于记录用户操作日志
    /// </summary>
    public class CmsOperationLog : BaseEntity
    {
        /// <summary>
        /// 用户 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户对象
        /// </summary>
        public CmsUser User { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string OperationType { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        public string OperationContent { get; set; }

        /// <summary>
        /// IP 地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 用户代理
        /// </summary>
        public string UserAgent { get; set; }
    }
}