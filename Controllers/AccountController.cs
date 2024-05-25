using Microsoft.AspNetCore.Mvc;
using EmployeeCensus.Data;
using EmployeeCensus.Models;
using EmployeeCensus.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System;

namespace EmployeeCensus.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;

        // Конструктор, принимающий экземпляр DataContext через DI
        public AccountController(DataContext context)
        {
            _context = context;
        }

        // Метод для отображения страницы логина
        [HttpGet]
        public IActionResult Login(bool success = false)
        {
            ViewBag.InvalidLogin = false;
            ViewBag.Success = success;
            return View();
        }

        // Метод для обработки POST-запроса логина
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                // Поиск пользователя по имени
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
                // Проверка хеша пароля
                if (user != null && PasswordHasher.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                {
                    // Создание claims для аутентификации
                    var claims = new[] {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Вход пользователя в систему
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Employees");
                }

                // Если логин неудачен, возвращаем ошибку
                ModelState.AddModelError("", "Invalid username or password");
                ViewBag.InvalidLogin = true;
            }

            return View();
        }

        // Метод для отображения страницы регистрации
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Метод для обработки POST-запроса регистрации
        [HttpPost]
        public async Task<IActionResult> Register(User user, string password)
        {
            if (ModelState.IsValid)
            {
                // Создание хеша и соли для пароля
                PasswordHasher.CreatePasswordHash(password, out string passwordHash, out string passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.LastActionTime = DateTime.UtcNow;

                // Добавление пользователя в базу данных
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", new { success = true });
            }
            return View(user);
        }

        // Метод для выхода из системы
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
