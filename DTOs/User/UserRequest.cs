using ArduinoAttendance.Domain.Enum;

namespace ArduinoAttendance.DTOs.User
{
    public class UserRequest
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Nic { get; set; }
        public string RFIDTag { get; set; }
        public Role Role { get; set; }
    }
}
