using ArduinoAttendance.Domain.Entities;
using ArduinoAttendance.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;


namespace ArduinoAttendance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendancesController : ControllerBase
    {
        private readonly AttendanceService _attendanceService;

        public AttendancesController(AttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _attendanceService.GetAllAttendancesAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var attendance = await _attendanceService.GetAttendanceByIdAsync(id);
            if (attendance == null) return NotFound();
            return Ok(attendance);
        }

        [HttpGet("by-date")]
        public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
        {
            return Ok(await _attendanceService.GetAttendancesByDateAsync(date));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Attendance updatedAttendance)
        {
            var attendance = await _attendanceService.UpdateAttendanceAsync(id, updatedAttendance);
            if (attendance == null) return NotFound();
            return Ok(attendance);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _attendanceService.DeleteAttendanceAsync(id);
            if (!result) return NotFound();
            return Ok("Deleted successfully");
        }
    }
}
