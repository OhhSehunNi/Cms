using Cms.Application.Services;
using Cms.Application.Services.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cms.Admin.Controllers
{
    /// <summary>
    /// 标签管理控制器
    /// </summary>
    public class TagController : Controller
    {
        private readonly ITagService _tagService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tagService">标签服务</param>
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// 标签列表页面
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="keyword">关键词</param>
        /// <returns>标签列表视图</returns>
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string keyword = null)
        {
            // 默认使用网站ID为1
            int websiteId = 1;
            var tags = await _tagService.GetListAsync(page, pageSize, keyword, websiteId);
            ViewBag.Keyword = keyword;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            return View(tags);
        }

        /// <summary>
        /// 创建标签页面
        /// </summary>
        /// <returns>创建标签视图</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 提交创建标签
        /// </summary>
        /// <param name="tagDto">标签DTO</param>
        /// <returns>重定向到标签列表</returns>
        [HttpPost]
        public async Task<IActionResult> Create(TagDto tagDto)
        {
            if (ModelState.IsValid)
            {
                // 默认使用网站ID为1
                tagDto.WebsiteId = 1;
                await _tagService.CreateAsync(tagDto);
                return RedirectToAction(nameof(Index));
            }
            return View(tagDto);
        }

        /// <summary>
        /// 编辑标签页面
        /// </summary>
        /// <param name="id">标签ID</param>
        /// <returns>编辑标签视图</returns>
        public async Task<IActionResult> Edit(int id)
        {
            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        /// <summary>
        /// 提交编辑标签
        /// </summary>
        /// <param name="id">标签ID</param>
        /// <param name="tagDto">标签DTO</param>
        /// <returns>重定向到标签列表</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TagDto tagDto)
        {
            if (id != tagDto.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _tagService.UpdateAsync(tagDto);
                return RedirectToAction(nameof(Index));
            }
            return View(tagDto);
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="id">标签ID</param>
        /// <returns>重定向到标签列表</returns>
        public async Task<IActionResult> Delete(int id)
        {
            await _tagService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}