namespace Cms.Application.Services
{
    public interface IOperationLogService
    {
        Task<List<OperationLogDto>> GetListAsync(int page, int pageSize, string? keyword = null, string? operationType = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<OperationLogDto> GetByIdAsync(int id);
        Task CreateLogAsync(string operationType, string content, string userId, string ipAddress);
    }

    public class OperationLogDto
    {
        public int Id { get; set; }
        public string OperationType { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}