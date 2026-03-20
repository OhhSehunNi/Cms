using Cms.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 操作日志控制器
    /// 提供操作日志的查询、导出和清理功能
    /// </summary>
    [Route("api/operation-logs")]
    [ApiController]
    [Authorize]
    public class OperationLogController : ControllerBase
    {
        private readonly IOperationLogService _operationLogService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="operationLogService">操作日志服务实例</param>
        public OperationLogController(IOperationLogService operationLogService)
        {
            _operationLogService = operationLogService;
        }

        /// <summary>
        /// 获取操作日志列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页大小，默认20</param>
        /// <param name="keyword">关键词</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="userId">用户ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>操作日志列表和总数</returns>
        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? keyword = null,
            [FromQuery] string? operationType = null,
            [FromQuery] int? userId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var logs = await _operationLogService.GetListAsync(page, pageSize, keyword, operationType, userId, startDate, endDate);
                var total = await _operationLogService.GetCountAsync(keyword, operationType, userId, startDate, endDate);

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
                    message = "获取操作日志列表成功"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取操作日志详情
        /// </summary>
        /// <param name="id">日志ID</param>
        /// <returns>操作日志详情</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var log = await _operationLogService.GetByIdAsync(id);
                if (log == null)
                {
                    return NotFound(new { success = false, message = "操作日志不存在" });
                }

                return Ok(new { success = true, data = log, message = "获取操作日志详情成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 获取所有操作类型列表
        /// </summary>
        /// <returns>操作类型列表</returns>
        [HttpGet("types")]
        public async Task<IActionResult> GetOperationTypes()
        {
            try
            {
                var types = await _operationLogService.GetOperationTypesAsync();
                return Ok(new { success = true, data = types, message = "获取操作类型列表成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 导出操作日志
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="userId">用户ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>导出的日志数据</returns>
        [HttpPost("export")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> ExportLogs(
            [FromQuery] string? keyword = null,
            [FromQuery] string? operationType = null,
            [FromQuery] int? userId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                // 导出所有符合条件的日志，不分页
                var logs = await _operationLogService.GetListAsync(1, 10000, keyword, operationType, userId, startDate, endDate);

                return Ok(new
                {
                    success = true,
                    data = logs,
                    message = "导出操作日志成功"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 清理过期操作日志
        /// </summary>
        /// <param name="days">保留天数，默认90天</param>
        /// <returns>清理的日志数量</returns>
        [HttpDelete("clear")]
        [Authorize(Roles = "超级管理员")]
        public async Task<IActionResult> ClearOldLogs([FromQuery] int days = 90)
        {
            try
            {
                var count = await _operationLogService.ClearOldLogsAsync(days);
                return Ok(new
                {
                    success = true,
                    data = new { clearedCount = count },
                    message = $"成功清理 {count} 条过期操作日志"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
