using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet("/article/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            // 增加浏览量
            await _articleService.IncrementViewCountAsync(id);

            return View(article);
        }

        [HttpGet("/channel/{channelId}")]
        public async Task<IActionResult> Channel(int channelId, int page = 1)
        {
            var articles = await _articleService.GetListAsync(page, 10, null, channelId);
            return View(articles);
        }

        [HttpGet("/search")]
        public async Task<IActionResult> Search(string keyword, int page = 1)
        {
            var articles = await _articleService.GetListAsync(page, 10, keyword);
            ViewBag.Keyword = keyword;
            return View(articles);
        }
    }
}