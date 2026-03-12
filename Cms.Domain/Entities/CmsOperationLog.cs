namespace Cms.Domain.Entities
{
    public class CmsOperationLog : BaseEntity
    {
        public int UserId { get; set; }
        public CmsUser User { get; set; }
        public string OperationType { get; set; }
        public string OperationContent { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}