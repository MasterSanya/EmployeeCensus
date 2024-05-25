namespace EmployeeCensus.Models
{
    public class EmployeeProgrammingLanguage
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int ProgrammingLanguageId { get; set; }
        public ProgrammingLanguage ProgrammingLanguage { get; set; }
    }
}
