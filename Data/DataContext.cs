using Microsoft.EntityFrameworkCore;
using EmployeeCensus.Models;

namespace EmployeeCensus.Data
{
    public class DataContext : DbContext
    {
        // Конструктор, принимающий параметры конфигурации через DI
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // Определение DbSet для каждой модели
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }
        public DbSet<EmployeeProgrammingLanguage> EmployeeProgrammingLanguages { get; set; }
        public DbSet<User> Users { get; set; }

        // Переопределение метода для настройки моделей при создании
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Установка составного ключа для связующей таблицы EmployeeProgrammingLanguage
            modelBuilder.Entity<EmployeeProgrammingLanguage>()
                .HasKey(epl => new { epl.EmployeeId, epl.ProgrammingLanguageId });

            // Установка уникального индекса для имени пользователя в таблице User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Предзаполнение таблицы Department начальными данными
            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, Name = "IT", Floor = 5 },
                new Department { DepartmentId = 2, Name = "HR", Floor = 3 },
                new Department { DepartmentId = 3, Name = "Finance", Floor = 4 },
                new Department { DepartmentId = 4, Name = "Marketing", Floor = 6 },
                new Department { DepartmentId = 5, Name = "Sales", Floor = 7 },
                new Department { DepartmentId = 6, Name = "Customer Support", Floor = 2 },
                new Department { DepartmentId = 7, Name = "Research and Development", Floor = 8 },
                new Department { DepartmentId = 8, Name = "Product Management", Floor = 5 },
                new Department { DepartmentId = 9, Name = "Quality Assurance", Floor = 4 },
                new Department { DepartmentId = 10, Name = "DevOps", Floor = 3 }
            );

            // Предзаполнение таблицы ProgrammingLanguage начальными данными
            modelBuilder.Entity<ProgrammingLanguage>().HasData(
                new ProgrammingLanguage { ProgrammingLanguageId = 1, Name = "C++" },
                new ProgrammingLanguage { ProgrammingLanguageId = 2, Name = "C#" },
                new ProgrammingLanguage { ProgrammingLanguageId = 3, Name = "Java" },
                new ProgrammingLanguage { ProgrammingLanguageId = 4, Name = "Python" },
                new ProgrammingLanguage { ProgrammingLanguageId = 5, Name = "PHP" },
                new ProgrammingLanguage { ProgrammingLanguageId = 6, Name = "JavaScript" },
                new ProgrammingLanguage { ProgrammingLanguageId = 7, Name = "Ruby" },
                new ProgrammingLanguage { ProgrammingLanguageId = 8, Name = "Go" },
                new ProgrammingLanguage { ProgrammingLanguageId = 9, Name = "Swift" },
                new ProgrammingLanguage { ProgrammingLanguageId = 10, Name = "Kotlin" }
            );
        }
    }
}
