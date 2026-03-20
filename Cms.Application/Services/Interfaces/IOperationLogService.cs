namespace Cms.Application.Services
{
    /// <summary>
    /// 操作日志服务接口，用于操作日志相关的业务逻辑
    /// </summary>
    public interface IOperationLogService
    {
        /// <summary>
        /// 获取操作日志列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="userId">用户ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>操作日志 DTO 列表</returns>
        Task<List<OperationLogDto>> GetListAsync(int page, int pageSize, string? keyword = null, string? operationType = null, int? userId = null, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// 获取操作日志总数
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="userId">用户ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>操作日志总数</returns>
        Task<int> GetCountAsync(string? keyword = null, string? operationType = null, int? userId = null, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// 根据 ID 获取操作日志
        /// </summary>
        /// <param name="id">日志 ID</param>
        /// <returns>操作日志 DTO</returns>
        Task<OperationLogDto> GetByIdAsync(int id);

        /// <summary>
        /// 创建操作日志
        /// </summary>
        /// <param name="operationType">操作类型</param>
        /// <param name="content">操作内容</param>
        /// <param name="userId">用户 ID</param>
        /// <param name="ipAddress">IP 地址</param>
        /// <param name="userAgent">用户代理</param>
        /// <returns></returns>
        Task CreateLogAsync(string operationType, string content, int userId, string ipAddress, string? userAgent = null);

        /// <summary>
        /// 获取所有操作类型列表
        /// </summary>
        /// <returns>操作类型列表</returns>
        Task<List<string>> GetOperationTypesAsync();

        /// <summary>
        /// 清理过期日志
        /// </summary>
        /// <param name="days">保留天数</param>
        /// <returns>清理的日志数量</returns>
        Task<int> ClearOldLogsAsync(int days);
    }

    /// <summary>
    /// 操作日志数据传输对象，用于操作日志相关的请求和响应
    /// </summary>
    public class OperationLogDto
    {
        /// <summary>
        /// 日志 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string OperationType { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 用户 ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// IP 地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
