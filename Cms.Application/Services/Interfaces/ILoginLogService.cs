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
        Task LogLoginAsync(int? userId, string username, string ip, string userAgent, bool status, string message);
        Task<List<CmsLoginLog>> GetRecentLoginLogsAsync(int userId, int count = 10);
    }
}