using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cms.Application.Services
{
    /// <summary>
    /// 登录日志服务，用于记录用户登录相关的日志
    /// </summary>
    public class LoginLogService : ILoginLogService
    {
        private readonly CmsDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public LoginLogService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 记录登录日志
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="username">用户名</param>
        /// <param name="ip">IP地址</param>
        /// <param name="userAgent">用户代理</param>
        /// <param name="status">登录状态</param>
        /// <param name="message">登录消息</param>
        /// <returns></returns>
        public async Task LogLoginAsync(int? userId, string username, string ip, string userAgent, bool status, string message)
        {
            var loginLog = new CmsLoginLog
            {
                UserId = userId,
                Username = username,
                Ip = ip,
                UserAgent = userAgent,
                Status = status,
                Message = message,
                CreatedAt = DateTime.Now
            };

            _dbContext.CmsLoginLogs.Add(loginLog);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 获取用户最近的登录日志
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="count">日志数量</param>
        /// <returns>登录日志列表</returns>
        public async Task<List<CmsLoginLog>> GetRecentLoginLogsAsync(int userId, int count = 10)
        {
            return await _dbContext.CmsLoginLogs
                .Where(log => log.UserId == userId && !log.IsDeleted)
                .OrderByDescending(log => log.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// 获取登录日志列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="username">用户名</param>
        /// <param name="ip">IP地址</param>
        /// <param name="status">登录状态</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>登录日志列表</returns>
        public async Task<List<LoginLogDto>> GetLoginLogListAsync(int page, int pageSize, string? username = null, string? ip = null, bool? status = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            IQueryable<CmsLoginLog> query = _dbContext.CmsLoginLogs.Where(l => !l.IsDeleted);

            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(log => log.Username.Contains(username));
            }

            if (!string.IsNullOrEmpty(ip))
            {
                query = query.Where(log => log.Ip.Contains(ip));
            }

            if (status.HasValue)
            {
                query = query.Where(log => log.Status == status.Value);
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
        /// 获取登录日志总数
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="ip">IP地址</param>
        /// <param name="status">登录状态</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>登录日志总数</returns>
        public async Task<int> GetLoginLogCountAsync(string? username = null, string? ip = null, bool? status = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            IQueryable<CmsLoginLog> query = _dbContext.CmsLoginLogs.Where(l => !l.IsDeleted);

            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(log => log.Username.Contains(username));
            }

            if (!string.IsNullOrEmpty(ip))
            {
                query = query.Where(log => log.Ip.Contains(ip));
            }

            if (status.HasValue)
            {
                query = query.Where(log => log.Status == status.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(log => log.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(log => log.CreatedAt <= endDate.Value);
            }

            return await query.CountAsync();
        }

        /// <summary>
        /// 获取指定用户的登录日志
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>登录日志列表</returns>
        public async Task<List<LoginLogDto>> GetUserLoginLogsAsync(int userId, int page, int pageSize)
        {
            var logs = await _dbContext.CmsLoginLogs
                .Where(log => log.UserId == userId && !log.IsDeleted)
                .OrderByDescending(log => log.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return logs.Select(MapToDto).ToList();
        }

        /// <summary>
        /// 获取登录统计信息
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>登录统计信息</returns>
        public async Task<LoginStatisticsDto> GetLoginStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            IQueryable<CmsLoginLog> query = _dbContext.CmsLoginLogs.Where(l => !l.IsDeleted);

            if (startDate.HasValue)
            {
                query = query.Where(log => log.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(log => log.CreatedAt <= endDate.Value);
            }

            var totalLogins = await query.CountAsync();
            var successLogins = await query.Where(log => log.Status).CountAsync();
            var failedLogins = await query.Where(log => !log.Status).CountAsync();
            var uniqueUsers = await query.Select(log => log.UserId).Distinct().CountAsync();

            var today = DateTime.Today;
            var todayLogins = await _dbContext.CmsLoginLogs
                .Where(log => !log.IsDeleted && log.CreatedAt >= today && log.CreatedAt < today.AddDays(1))
                .CountAsync();

            return new LoginStatisticsDto
            {
                TotalLogins = totalLogins,
                SuccessLogins = successLogins,
                FailedLogins = failedLogins,
                UniqueUsers = uniqueUsers,
                TodayLogins = todayLogins
            };
        }

        /// <summary>
        /// 清理过期登录日志
        /// </summary>
        /// <param name="days">保留天数</param>
        /// <returns>清理的日志数量</returns>
        public async Task<int> ClearOldLogsAsync(int days)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            var oldLogs = await _dbContext.CmsLoginLogs
                .Where(l => l.CreatedAt < cutoffDate && !l.IsDeleted)
                .ToListAsync();

            foreach (var log in oldLogs)
            {
                log.IsDeleted = true;
                log.UpdatedAt = DateTime.Now;
            }

            await _dbContext.SaveChangesAsync();
            return oldLogs.Count;
        }

        /// <summary>
        /// 将实体映射为DTO
        /// </summary>
        /// <param name="log">登录日志实体</param>
        /// <returns>登录日志DTO</returns>
        private LoginLogDto MapToDto(CmsLoginLog log)
        {
            return new LoginLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                Username = log.Username,
                Ip = log.Ip,
                UserAgent = log.UserAgent,
                Status = log.Status,
                Message = log.Message,
                CreatedAt = log.CreatedAt
            };
        }
    }
}
