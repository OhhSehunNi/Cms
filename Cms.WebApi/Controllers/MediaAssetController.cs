using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    /// <summary>
    /// 媒体资源控制器
    /// 提供媒体资源的上传、获取、删除以及获取资源分组等功能
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MediaAssetController : ControllerBase
    {
        /// <summary>
        /// 媒体资源服务接口
        /// </summary>
        private readonly IMediaAssetService _mediaAssetService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mediaAssetService">媒体资源服务实例</param>
        public MediaAssetController(IMediaAssetService mediaAssetService)
        {
            _mediaAssetService = mediaAssetService;
        }

        /// <summary>
        /// 根据ID获取媒体资源信息
        /// </summary>
        /// <param name="id">媒体资源ID</param>
        /// <returns>媒体资源信息</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var asset = await _mediaAssetService.GetByIdAsync(id);
            if (asset == null)
            {
                return NotFound();
            }
            return Ok(asset);
        }

        /// <summary>
        /// 获取媒体资源列表
        /// </summary>
        /// <param name="page">页码，默认1</param>
        /// <param name="pageSize">每页数量，默认10</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="group">资源分组</param>
        /// <returns>媒体资源列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null, string? group = null)
        {
            var assets = await _mediaAssetService.GetListAsync(page, pageSize, keyword, group);
            return Ok(assets);
        }

        /// <summary>
        /// 上传媒体资源
        /// </summary>
        /// <param name="file">上传的文件</param>
        /// <param name="group">资源分组</param>
        /// <returns>上传的媒体资源信息</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, string? group = null)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var fileData = memoryStream.ToArray();

                var asset = await _mediaAssetService.UploadAsync(
                    file.FileName,
                    file.ContentType,
                    file.Length,
                    fileData,
                    group
                );

                return Ok(asset);
            }
        }

        /// <summary>
        /// 删除媒体资源
        /// </summary>
        /// <param name="id">媒体资源ID</param>
        /// <returns>无内容</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediaAssetService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// 获取媒体资源分组
        /// </summary>
        /// <returns>资源分组列表</returns>
        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups()
        {
            var groups = await _mediaAssetService.GetGroupsAsync();
            return Ok(groups);
        }
    }
}