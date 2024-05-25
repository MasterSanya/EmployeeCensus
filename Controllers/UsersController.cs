using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeCensus.Data;
using EmployeeCensus.Models;
using System.Threading.Tasks;
using System;
using EmployeeCensus.Services;

namespace EmployeeCensus.Controllers
{
    public class UsersController : Controller
    {
        private readonly DataContext _context;

        // Конструктор, принимающий экземпляр DataContext через DI
        public UsersController(DataContext context)
        {
            _context = context;
        }

        // Метод для отображения страницы добавления пользователя
        public IActionResult Add()
        {
            return View();
        }

        // Метод для обработки POST-запроса добавления пользователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(User user, string password)
        {
            if (ModelState.IsValid)
            {
                // Создание хеша и соли для пароля
                PasswordHasher.CreatePasswordHash(password, out string passwordHash, out string passwordSalt);

                // Установка хеша и соли пароля, а также времени последнего действия
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.LastActionTime = DateTime.UtcNow;

                // Добавление пользователя в базу данных
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // Метод для отображения списка пользователей
        public async Task<IActionResult> Index()
        {
            // Получение списка всех пользователей
            var users = await _context.Users.ToListAsync();
            return View(users);
        }
    }
}