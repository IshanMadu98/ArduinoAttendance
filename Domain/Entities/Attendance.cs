using ArduinoAttendance.Domain.Entities.Common;
using ArduinoAttendance.Domain.Enum;

namespace ArduinoAttendance.Domain.Entities
{
    public class Attendance : Entity
    {
        public DateOnly SignedInDate { get; set; }
        public TimeOnly SignedInTime { get; set; }
        public DateTime SignedInDateTime { get; set; }
        public DateOnly? SignedOutDate { get; set; }
        public TimeOnly? SignedOutTime { get; set; }
        public DateTime? SignedOutDateTime { get; set; }
        public SignedStatus Status { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
