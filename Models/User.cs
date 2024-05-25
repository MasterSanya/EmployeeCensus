using System;

namespace EmployeeCensus.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime LastActionTime { get; set; }
    }
}
