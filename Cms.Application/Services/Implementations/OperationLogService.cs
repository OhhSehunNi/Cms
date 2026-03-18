using Cms.Application.Services;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cms.Application.Services
{
    /// <summary>
    /// 操作日志服务实现类，用于操作日志相关的业务逻辑
    /// </summary>
    public class OperationLogService : IOperationLogService
    {
        private readonly CmsDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public OperationLogService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取操作日志列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>操作日志 DTO 列表</returns>
        public async Task<List<OperationLogDto>> GetListAsync(int page, int pageSize, string? keyword = null, string? operationType = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            IQueryable<CmsOperationLog> query = _dbContext.CmsOperationLogs;

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(log => log.OperationContent.Contains(keyword) || log.UserId.ToString().Contains(keyword) || log.IpAddress.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(operationType))
            {
                query = query.Where(log => log.OperationType == operationType);
            }

            if (startDate.HasValue)
            {
                query = query.Where(log => log.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(log => log.CreatedAt <= endDate.Value);
            }

            var logs = await query
                .OrderByDescending(log => log.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return logs.Select(MapToDto).ToList();
        }

        /// <summary>
        /// 根据 ID 获取操作日志
        /// </summary>
        /// <param name="id">日志 ID</param>
        /// <returns>操作日志 DTO</returns>
        public async Task<OperationLogDto> GetByIdAsync(int id)
        {
            var log = await _dbContext.CmsOperationLogs.FindAsync(id);
            if (log == null)
                return null;

            return MapToDto(log);
        }

        /// <summary>
        /// 创建操作日志
        /// </summary>
        /// <param name="operationType">操作类型</param>
        /// <param name="content">操作内容</param>
        /// <param name="userId">用户 ID</param>
        /// <param name="ipAddress">IP 地址</param>
        /// <returns></returns>
        public async Task CreateLogAsync(string operationType, string content, string userId, string ipAddress)
        {
            var log = new CmsOperationLog
            {
                OperationType = operationType,
                OperationContent = content,
                UserId = int.Parse(userId),
                IpAddress = ipAddress,
                CreatedAt = DateTime.Now
            };

            _dbContext.CmsOperationLogs.Add(log);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 将实体映射为 DTO
        /// </summary>
        /// <param name="log">操作日志实体</param>
        /// <returns>操作日志 DTO</returns>
        private OperationLogDto MapToDto(CmsOperationLog log)
        {
            return new OperationLogDto
            {
                Id = log.Id,
                OperationType = log.OperationType,
                Content = log.OperationContent,
                UserId = log.UserId.ToString(),
                IpAddress = log.IpAddress,
                CreatedAt = log.CreatedAt
            };
        }
    }
}
