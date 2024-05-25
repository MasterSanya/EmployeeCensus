using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeCensus.Data;
using EmployeeCensus.Models;
using EmployeeCensus.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;

namespace EmployeeCensus.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly DataContext _context;

        // Конструктор, принимающий экземпляр DataContext через DI
        public EmployeesController(DataContext context)
        {
            _context = context;
        }

        // Метод для отображения списка сотрудников с поиском
        public async Task<IActionResult> Index(string search)
        {
            // Получение списка сотрудников, которые не удалены
            var employees = from e in _context.Employees
                            .Include(e => e.Department)
                            .Where(e => !e.IsDeleted)
                            select new EmployeeViewModel
                            {
                                EmployeeId = e.EmployeeId,
                                FirstName = e.FirstName,
                                LastName = e.LastName,
                                Age = e.Age,
                                Gender = e.Gender,
                                DepartmentId = e.DepartmentId,
                                DepartmentName = e.Department.Name
                            };

            // Фильтрация списка по имени или фамилии, если задан поиск
            if (!string.IsNullOrEmpty(search))
            {
                employees = employees.Where(e => e.FirstName.Contains(search) || e.LastName.Contains(search));
            }

            // Возвращение представления с полученным списком сотрудников
            return View(await employees.ToListAsync());
        }

        // Метод для отображения страницы добавления сотрудника (только для авторизованных пользователей)
        [Authorize(Policy = "RequireAuthenticatedUser")]
        public IActionResult Add()
        {
            // Создание модели представления с выборками отделов и языков программирования
            var viewModel = new EmployeeViewModel
            {
                Departments = new SelectList(_context.Departments, "DepartmentId", "Name"),
                ProgrammingLanguages = new SelectList(_context.ProgrammingLanguages, "ProgrammingLanguageId", "Name")
            };

            return View(viewModel);
        }

        // Метод для обработки POST-запроса добавления сотрудника (только для авторизованных пользователей)
        [HttpPost]
        [Authorize(Policy = "RequireAuthenticatedUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(EmployeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var employee = new Employee
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Age = viewModel.Age,
                    Gender = viewModel.Gender,
                    DepartmentId = viewModel.DepartmentId,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedBy = userId,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Add(employee);
                await _context.SaveChangesAsync();

                // Добавление выбранных языков программирования для сотрудника
                if (viewModel.SelectedProgrammingLanguages != null)
                {
                    foreach (var langId in viewModel.SelectedProgrammingLanguages)
                    {
                        _context.EmployeeProgrammingLanguages.Add(new EmployeeProgrammingLanguage
                        {
                            EmployeeId = employee.EmployeeId,
                            ProgrammingLanguageId = langId
                        });
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // Повторная инициализация выборок при ошибке валидации
            viewModel.Departments = new SelectList(_context.Departments, "DepartmentId", "Name");
            viewModel.ProgrammingLanguages = new SelectList(_context.ProgrammingLanguages, "ProgrammingLanguageId", "Name");
            return View(viewModel);
        }

        // Метод для отображения страницы редактирования сотрудника (только для авторизованных пользователей)
        [Authorize(Policy = "RequireAuthenticatedUser")]
        public async Task<IActionResult> Edit(int id)
        {
            // Поиск сотрудника по ID
            var employee = await _context.Employees
                .Include(e => e.EmployeeProgrammingLanguages)
                .Include(e => e.Department)
                .SingleOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Создание модели представления для редактирования
            var viewModel = new EmployeeViewModel
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Age = employee.Age,
                Gender = employee.Gender,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department.Name,
                Departments = new SelectList(_context.Departments, "DepartmentId", "Name", employee.DepartmentId),
                SelectedProgrammingLanguages = employee.EmployeeProgrammingLanguages.Select(epl => epl.ProgrammingLanguageId).ToList(),
                ProgrammingLanguages = new SelectList(_context.ProgrammingLanguages, "ProgrammingLanguageId", "Name")
            };

            return View(viewModel);
        }

        // Метод для обработки POST-запроса редактирования сотрудника (только для авторизованных пользователей)
        [HttpPost]
        [Authorize(Policy = "RequireAuthenticatedUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Поиск сотрудника по ID
                var employee = await _context.Employees
                    .Include(e => e.EmployeeProgrammingLanguages)
                    .SingleOrDefaultAsync(e => e.EmployeeId == viewModel.EmployeeId);

                if (employee == null)
                {
                    return NotFound();
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                // Обновление данных сотрудника
                employee.FirstName = viewModel.FirstName;
                employee.LastName = viewModel.LastName;
                employee.Age = viewModel.Age;
                employee.Gender = viewModel.Gender;
                employee.DepartmentId = viewModel.DepartmentId;
                employee.UpdatedBy = userId;
                employee.UpdatedAt = DateTime.UtcNow;

                // Удаление всех записей о языках программирования сотрудника
                _context.EmployeeProgrammingLanguages.RemoveRange(employee.EmployeeProgrammingLanguages);

                // Добавление новых записей о языках программирования
                if (viewModel.SelectedProgrammingLanguages != null)
                {
                    foreach (var langId in viewModel.SelectedProgrammingLanguages)
                    {
                        _context.EmployeeProgrammingLanguages.Add(new EmployeeProgrammingLanguage
                        {
                            EmployeeId = employee.EmployeeId,
                            ProgrammingLanguageId = langId
                        });
                    }
                }

                _context.Update(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Повторная инициализация выборок при ошибке валидации
            viewModel.Departments = new SelectList(_context.Departments, "DepartmentId", "Name", viewModel.DepartmentId);
            viewModel.ProgrammingLanguages = new SelectList(_context.ProgrammingLanguages, "ProgrammingLanguageId", "Name");
            return View(viewModel);
        }

        // Метод для отображения страницы удаления сотрудника (только для авторизованных пользователей)
        [Authorize(Policy = "RequireAuthenticatedUser")]
        public async Task<IActionResult> Delete(int id)
        {
            // Поиск сотрудника по ID
            var employee = await _context.Employees
                .Include(e => e.Department)
                .SingleOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Создание модели представления для удаления
            var viewModel = new EmployeeViewModel
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Age = employee.Age,
                Gender = employee.Gender,
                DepartmentName = employee.Department.Name,
                DepartmentId = employee.DepartmentId,
                Departments = new SelectList(_context.Departments, "DepartmentId", "Name", employee.DepartmentId)
            };

            return View(viewModel);
        }

        // Метод для обработки POST-запроса удаления сотрудника (только для авторизованных пользователей)
        [HttpPost, ActionName("DeleteConfirmed")]
        [Authorize(Policy = "RequireAuthenticatedUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("EmployeeId")] EmployeeViewModel viewModel)
        {
            System.Diagnostics.Debug.WriteLine($"DeleteConfirmed called with ID: {viewModel.EmployeeId}");

            var employee = await _context.Employees.FindAsync(viewModel.EmployeeId);
            if (employee != null)
            {
                System.Diagnostics.Debug.WriteLine($"Found employee with ID: {employee.EmployeeId}");

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                employee.IsDeleted = true;
                employee.UpdatedBy = userId;
                employee.UpdatedAt = DateTime.UtcNow;

                _context.Update(employee);
                await _context.SaveChangesAsync();

                System.Diagnostics.Debug.WriteLine($"Employee {employee.EmployeeId} marked as deleted.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Employee with ID: {viewModel.EmployeeId} not found.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
