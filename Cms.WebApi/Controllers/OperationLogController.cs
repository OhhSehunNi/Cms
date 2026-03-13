using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationLogController : ControllerBase
    {
        private readonly IOperationLogService _operationLogService;

        public OperationLogController(IOperationLogService operationLogService)
        {
            _operationLogService = operationLogService;
        }

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

        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null, string? operationType = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var logs = await _operationLogService.GetListAsync(page, pageSize, keyword, operationType, startDate, endDate);
            return Ok(logs);
        }
    }
}