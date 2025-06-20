using ArduinoAttendance.Domain.Entities;
using ArduinoAttendance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArduinoAttendance.Infrastructure.Services
{
    public class AttendanceService
    {
        private readonly AppDbContext _dbContext;

        public AttendanceService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Attendance>> GetAllAttendancesAsync()
        {
            return await _dbContext.Attendances.Include(a => a.User).ToListAsync();
        }

        public async Task<Attendance> GetAttendanceByIdAsync(Guid id)
        {
            return await _dbContext.Attendances.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Attendance>> GetAttendancesByDateAsync(DateTime date)
        {
            return await _dbContext.Attendances.Include(a => a.User)
                .Where(a => a.SignedInDateTime.Date == date.Date)
                .ToListAsync();
        }

        public async Task<Attendance> UpdateAttendanceAsync(Guid id, Attendance updatedAttendance)
        {
            var attendance = await _dbContext.Attendances.FindAsync(id);
            if (attendance == null) return null;

            attendance.UserId = updatedAttendance.UserId;
            attendance.SignedInDate = updatedAttendance.SignedInDate;
            attendance.SignedInTime = updatedAttendance.SignedInTime;
            attendance.SignedInDateTime = updatedAttendance.SignedInDateTime;
            attendance.SignedOutDate = updatedAttendance.SignedOutDate;
            attendance.SignedOutTime = updatedAttendance.SignedOutTime;
            attendance.SignedOutDateTime = updatedAttendance.SignedOutDateTime;
            attendance.Status = updatedAttendance.Status;

            await _dbContext.SaveChangesAsync();
            return attendance;
        }

        public async Task<bool> DeleteAttendanceAsync(Guid id)
        {
            var attendance = await _dbContext.Attendances.FindAsync(id);
            if (attendance == null) return false;

            _dbContext.Attendances.Remove(attendance);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
