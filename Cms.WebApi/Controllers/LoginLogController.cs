using Cms.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 登录日志控制器
    /// 提供登录日志的查询、统计和清理功能
    /// </summary>
    [Route("api/login-logs")]
    [ApiController]
    [Authorize]
    public class LoginLogController : ControllerBase
    {
        private readonly ILoginLogService _loginLogService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="loginLogService">登录日志服务实例</param>
        public LoginLogController(ILoginLogService loginLogService)
        {
            _loginLogService = loginLogService;
        }

        /// <summary>
        /// 获取登录日志列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页大小，默认20</param>
        /// <param name="username">用户名</param>
        /// <param name="ip">IP地址</param>
        /// <param name="status">登录状态</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>登录日志列表和总数</returns>
        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? username = null,
            [FromQuery] string? ip = null,
            [FromQuery] bool? status = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var logs = await _loginLogService.GetLoginLogListAsync(page, pageSize, username, ip, status, startDate, endDate);
                var total = await _loginLogService.GetLoginLogCountAsync(username, ip, status, startDate, endDate);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        list = logs,
                        total = total,
                        page = page,
                        pageSize = pageSize
                    },
                    message = "获取登录日志列表成功"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 获取当前用户的登录日志
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页大小，默认20</param>
        /// <returns>登录日志列表</returns>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyLoginLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var logs = await _loginLogService.GetUserLoginLogsAsync(userId, page, pageSize);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        list = logs,
                        page = page,
                        pageSize = pageSize
                    },
                    message = "获取登录日志成功"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 获取指定用户的登录日志
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页大小，默认20</param>
        /// <returns>登录日志列表</returns>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> GetUserLoginLogs(
            int userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var logs = await _loginLogService.GetUserLoginLogsAsync(userId, page, pageSize);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        list = logs,
                        page = page,
                        pageSize = pageSize
                    },
                    message = "获取用户登录日志成功"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 获取登录统计信息
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>登录统计信息</returns>
        [HttpGet("statistics")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> GetStatistics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var statistics = await _loginLogService.GetLoginStatisticsAsync(startDate, endDate);

                return Ok(new
                {
                    success = true,
                    data = statistics,
                    message = "获取登录统计信息成功"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 导出登录日志
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="ip">IP地址</param>
        /// <param name="status">登录状态</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>导出的日志数据</returns>
        [HttpPost("export")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> ExportLogs(
            [FromQuery] string? username = null,
            [FromQuery] string? ip = null,
            [FromQuery] bool? status = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                // 导出所有符合条件的日志，不分页
                var logs = await _loginLogService.GetLoginLogListAsync(1, 10000, username, ip, status, startDate, endDate);

                return Ok(new
                {
                    success = true,
                    data = logs,
                    message = "导出登录日志成功"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 清理过期登录日志
        /// </summary>
        /// <param name="days">保留天数，默认90天</param>
        /// <returns>清理的日志数量</returns>
        [HttpDelete("clear")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> ClearOldLogs([FromQuery] int days = 90)
        {
            try
            {
                var count = await _loginLogService.ClearOldLogsAsync(days);
                return Ok(new
                {
                    success = true,
                    data = new { clearedCount = count },
                    message = $"成功清理 {count} 条过期登录日志"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
