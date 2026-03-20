using Cms.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cms.Application.Services
{
    /// <summary>
    /// 登录日志服务接口
    /// </summary>
    public interface ILoginLogService
    {
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
        Task LogLoginAsync(int? userId, string username, string ip, string userAgent, bool status, string message);

        /// <summary>
        /// 获取用户最近的登录日志
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="count">日志数量</param>
        /// <returns>登录日志列表</returns>
        Task<List<CmsLoginLog>> GetRecentLoginLogsAsync(int userId, int count = 10);

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
        Task<List<LoginLogDto>> GetLoginLogListAsync(int page, int pageSize, string? username = null, string? ip = null, bool? status = null, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// 获取登录日志总数
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="ip">IP地址</param>
        /// <param name="status">登录状态</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>登录日志总数</returns>
        Task<int> GetLoginLogCountAsync(string? username = null, string? ip = null, bool? status = null, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// 获取指定用户的登录日志
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>登录日志列表</returns>
        Task<List<LoginLogDto>> GetUserLoginLogsAsync(int userId, int page, int pageSize);

        /// <summary>
        /// 获取登录统计信息
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>登录统计信息</returns>
        Task<LoginStatisticsDto> GetLoginStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// 清理过期登录日志
        /// </summary>
        /// <param name="days">保留天数</param>
        /// <returns>清理的日志数量</returns>
        Task<int> ClearOldLogsAsync(int days);
    }

    /// <summary>
    /// 登录日志数据传输对象
    /// </summary>
    public class LoginLogDto
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int? UserId { get; set; }

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

    /// <summary>
    /// 登录统计信息数据传输对象
    /// </summary>
    public class LoginStatisticsDto
    {
        /// <summary>
        /// 总登录次数
        /// </summary>
        public int TotalLogins { get; set; }

        /// <summary>
        /// 成功登录次数
        /// </summary>
        public int SuccessLogins { get; set; }

        /// <summary>
        /// 失败登录次数
        /// </summary>
        public int FailedLogins { get; set; }

        /// <summary>
        /// 独立用户数
        /// </summary>
        public int UniqueUsers { get; set; }

        /// <summary>
        /// 今日登录次数
        /// </summary>
        public int TodayLogins { get; set; }
    }
}