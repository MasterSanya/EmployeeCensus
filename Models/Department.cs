using System.Collections.Generic;

namespace EmployeeCensus.Models
{
    // Класс, представляющий отдел
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public int Floor { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
