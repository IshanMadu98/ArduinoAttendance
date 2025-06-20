using ArduinoAttendance.Domain.Entities;
using ArduinoAttendance.Domain.Enum;
using ArduinoAttendance.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ArduinoAttendance.SerialPortReader
{
    public class RFIDReaderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private SerialPort _serialPort;

        public RFIDReaderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _serialPort = new SerialPort("COM5", 9600)
            {
                ReadTimeout = 5000,
                WriteTimeout = 5000
            };

            _serialPort.DataReceived += SerialPort_DataReceived;

            _serialPort.Open();

            return Task.CompletedTask;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var sp = (SerialPort)sender;
                string rfidTag = sp.ReadLine().Trim();

                if (string.IsNullOrEmpty(rfidTag)) return;

                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var user = dbContext.Users.FirstOrDefault(u => u.RFIDTag == rfidTag);
                if (user == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"❌ [RFID Scan Failed] Unknown RFID: {rfidTag}");
                    Console.ResetColor();
                    _serialPort.WriteLine("FAIL");
                    return;
                }

                var today = DateTime.UtcNow.Date;

                // Get today's attendance if exists
                var todayAttendance = dbContext.Attendances
                    .Where(a => a.UserId == user.Id && a.SignedInDateTime.Date == today)
                    .OrderByDescending(a => a.SignedInDateTime)
                    .FirstOrDefault();

                if (todayAttendance == null)
                {
                    // First time scan today => Login
                    var attendance = new Attendance
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        SignedInDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        SignedInTime = TimeOnly.FromDateTime(DateTime.UtcNow),
                        SignedInDateTime = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        Status = SignedStatus.Login
                    };

                    dbContext.Attendances.Add(attendance);
                    dbContext.SaveChanges();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✅ [RFID Login] User: {user.FirstName} {user.LastName} | Time: {attendance.SignedInDateTime}");
                    Console.ResetColor();

                    _serialPort.WriteLine("LOGIN_OK");
                }
                else if (todayAttendance.Status == SignedStatus.Login && todayAttendance.SignedOutDateTime == null)
                {
                    // Second scan today => Logout
                    todayAttendance.SignedOutDate = DateOnly.FromDateTime(DateTime.UtcNow);
                    todayAttendance.SignedOutTime = TimeOnly.FromDateTime(DateTime.UtcNow);
                    todayAttendance.SignedOutDateTime = DateTime.UtcNow;
                    todayAttendance.LastModified = DateTime.UtcNow;
                    todayAttendance.Status = SignedStatus.Logout;

                    dbContext.Attendances.Update(todayAttendance);
                    dbContext.SaveChanges();

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"✅ [RFID Logout] User: {user.FirstName} {user.LastName} | Time: {todayAttendance.SignedOutDateTime}");
                    Console.ResetColor();

                    _serialPort.WriteLine("LOGOUT_OK");
                }
                else
                {
                    // Already signed in and signed out today — Block further attendance
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚠️ [RFID Scan Blocked] User: {user.FirstName} {user.LastName} already completed attendance today.");
                    Console.ResetColor();

                    _serialPort.WriteLine("ALREADY_DONE");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error reading RFID: " + ex.Message);
                Console.ResetColor();
            }
        }



        public override void Dispose()
        {
            _serialPort?.Close();
            _serialPort?.Dispose();
            base.Dispose();
        }
    }
}
