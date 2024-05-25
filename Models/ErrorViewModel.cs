namespace EmployeeCensus.Models
{
    public class ErrorViewModel
    {
        // Свойство для идентификатора запроса
        public string RequestId { get; set; }

        // Свойство, показывающее, пустой ли RequestId
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
