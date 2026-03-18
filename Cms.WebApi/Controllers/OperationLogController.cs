using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 操作日志控制器
    /// 提供操作日志的查询功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OperationLogController : ControllerBase
    {
        /// <summary>
        /// 操作日志服务接口
        /// </summary>
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
        /// 根据ID获取操作日志信息
        /// </summary>
        /// <param name="id">操作日志ID</param>
        /// <returns>操作日志信息</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var log = await _operationLogService.GetByIdAsync(id);
            if (log == null)
            {
                return NotFound();
            }
            return Ok(log);
        }

        /// <summary>
        /// 获取操作日志列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>操作日志列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null, string? operationType = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var logs = await _operationLogService.GetListAsync(page, pageSize, keyword, operationType, startDate, endDate);
            return Ok(logs);
        }
    }
}