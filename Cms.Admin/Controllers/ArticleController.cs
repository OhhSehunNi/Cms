using Cms.Application.Services.Dtos;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Admin.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet("/admin/articles")]
        public async Task<IActionResult> Index(int page = 1, string keyword = null, int? channelId = null)
        {
            var articles = await _articleService.GetListAsync(page, 20, keyword, channelId);
            ViewBag.Keyword = keyword;
            ViewBag.ChannelId = channelId;
            return View(articles);
        }

        [HttpGet("/admin/articles/create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("/admin/articles/create")]
        public async Task<IActionResult> Create(ArticleDto articleDto)
        {
            if (ModelState.IsValid)
            {
                await _articleService.CreateAsync(articleDto);
                return RedirectToAction("Index");
            }
            return View(articleDto);
        }

        [HttpGet("/admin/articles/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        [HttpPost("/admin/articles/edit/{id}")]
        public async Task<IActionResult> Edit(int id, ArticleDto articleDto)
        {
            if (ModelState.IsValid)
            {
                articleDto.Id = id;
                await _articleService.UpdateAsync(articleDto);
                return RedirectToAction("Index");
            }
            return View(articleDto);
        }

        [HttpPost("/admin/articles/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _articleService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost("/admin/articles/publish/{id}")]
        public async Task<IActionResult> Publish(int id)
        {
            await _articleService.PublishAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost("/admin/articles/unpublish/{id}")]
        public async Task<IActionResult> Unpublish(int id)
        {
            await _articleService.UnpublishAsync(id);
            return RedirectToAction("Index");
        }
    }
}