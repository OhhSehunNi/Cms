using Cms.Application.DTOs;
using Cms.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaAssetController : ControllerBase
    {
        private readonly IMediaAssetService _mediaAssetService;

        public MediaAssetController(IMediaAssetService mediaAssetService)
        {
            _mediaAssetService = mediaAssetService;
        }

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

        [HttpGet]
        public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? keyword = null, string? group = null)
        {
            var assets = await _mediaAssetService.GetListAsync(page, pageSize, keyword, group);
            return Ok(assets);
        }

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediaAssetService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups()
        {
            var groups = await _mediaAssetService.GetGroupsAsync();
            return Ok(groups);
        }
    }
}