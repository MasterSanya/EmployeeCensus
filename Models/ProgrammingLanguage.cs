using System.Collections.Generic;

namespace EmployeeCensus.Models
{
    // Класс, представляющий язык программирования
    public class ProgrammingLanguage
    {
        public int ProgrammingLanguageId { get; set; }
        public string Name { get; set; }
        public ICollection<EmployeeProgrammingLanguage> EmployeeProgrammingLanguages { get; set; }
    }
}
