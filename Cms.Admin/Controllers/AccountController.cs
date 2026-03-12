using Cms.Application.DTOs;
using Cms.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Admin.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("/admin/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("/admin/login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                var isValid = await _userService.ValidateCredentialsAsync(loginDto.Username, loginDto.Password);
                if (isValid)
                {
                    var user = await _userService.GetByUsernameAsync(loginDto.Username);
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("DisplayName", user.DisplayName);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "用户名或密码错误");
                }
            }
            return View(loginDto);
        }

        [HttpGet("/admin/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet("/admin/users")]
        public async Task<IActionResult> Index(int page = 1, string keyword = null)
        {
            var users = await _userService.GetListAsync(page, 20, keyword);
            ViewBag.Keyword = keyword;
            return View(users);
        }

        [HttpGet("/admin/users/create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("/admin/users/create")]
        public async Task<IActionResult> Create(RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                await _userService.CreateAsync(registerDto);
                return RedirectToAction("Index");
            }
            return View(registerDto);
        }

        [HttpGet("/admin/users/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost("/admin/users/edit/{id}")]
        public async Task<IActionResult> Edit(int id, UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                userDto.Id = id;
                await _userService.UpdateAsync(userDto);
                return RedirectToAction("Index");
            }
            return View(userDto);
        }

        [HttpPost("/admin/users/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}