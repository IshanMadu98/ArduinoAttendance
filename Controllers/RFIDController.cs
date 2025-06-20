using Microsoft.AspNetCore.Mvc;
using System;
using System.IO.Ports;

namespace ArduinoAttendance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RFIDController : ControllerBase
    {
        [HttpGet("read-rfid")]
        public IActionResult ReadRFID()
        {
            try
            {
                using var serialPort = new SerialPort("COM5", 9600)
                {
                    ReadTimeout = 3000, // 3 seconds timeout to avoid hanging forever
                    WriteTimeout = 3000
                };

                serialPort.Open();

                string rfidTag = serialPort.ReadLine().Trim();
                serialPort.Close();

                if (string.IsNullOrWhiteSpace(rfidTag))
                    return BadRequest("No RFID tag read.");

                return Ok(new { RFID = rfidTag });
            }
            catch (TimeoutException)
            {
                return StatusCode(500, "RFID read timed out.");
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(500, "COM port is busy or not accessible. Ensure no other app (like Arduino IDE) is using it.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error reading RFID: {ex.Message}");
            }
        }
    }
}
