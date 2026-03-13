using Cms.Application.Services;
using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cms.Application.Services
{
    public class OperationLogService : IOperationLogService
    {
        private readonly CmsDbContext _dbContext;

        public OperationLogService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

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

        public async Task<OperationLogDto> GetByIdAsync(int id)
        {
            var log = await _dbContext.CmsOperationLogs.FindAsync(id);
            if (log == null)
                return null;

            return MapToDto(log);
        }

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