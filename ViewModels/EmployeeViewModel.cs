using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeCensus.ViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Range(18, 65)]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } // Для отображения имени отдела

        public IEnumerable<int> SelectedProgrammingLanguages { get; set; }

        public IEnumerable<SelectListItem> Departments { get; set; }
        public IEnumerable<SelectListItem> ProgrammingLanguages { get; set; }
    }
}
