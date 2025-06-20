using System.Security.Principal;

namespace ArduinoAttendance.Domain.Entities.Common
{
    public abstract class Entity : BaseEntity
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModified { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

    }
}
