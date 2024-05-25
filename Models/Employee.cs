using System;
using System.Collections.Generic;

namespace EmployeeCensus.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<EmployeeProgrammingLanguage> EmployeeProgrammingLanguages { get; set; }
    }
}
