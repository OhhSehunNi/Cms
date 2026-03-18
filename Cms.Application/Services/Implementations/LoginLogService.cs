using Cms.Domain.Entities;
using Cms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}