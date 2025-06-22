# RFID Attendance System — .NET Core + Arduino Uno

A complete **RFID Attendance Tracking System** that reads RFID cards via Arduino Uno and marks **Login/Logout Attendance** into a PostgreSQL/SQL Server database using an ASP.NET Core backend.

---

## ⚙️ Tech Stack

| Layer                | Technology                        |
|---------------------|-----------------------------------|
| Backend             | **ASP.NET Core 8** + EF Core ORM   |
| Database           | PostgreSQL / SQL Server            |
| Arduino Hardware    | **Arduino Uno + MFRC522 RFID** Reader |
| Communication      | Serial Port (System.IO.Ports)      |
| Architecture       | Clean (Domain, Application, Infrastructure) |

---

## 📂 Project Structure

```
ArduinoAttendance
│
├── Application
│   └── Interfaces               // Service interfaces
│
├── Controllers
│   ├── AttendancesController.cs // Attendance APIs
│   ├── RFIDController.cs        // Manual RFID Read API
│   └── UsersController.cs       // User CRUD APIs
│
├── Domain
│   ├── Entities                // User.cs, Attendance.cs, etc.
│   ├── Enum                    // Enums (Roles, Status, etc.)
│   └── DTOs
│       └── User                // UserRequest DTO etc.
│
├── Infrastructure
│   ├── Data                    // AppDbContext
│   └── Services                // UserService, AttendanceService
│
├── Migrations                 // EF Core Migrations
│
├── SerialPortReader
│   └── RFIDReaderService.cs    // Background service handling RFID read via COM Port
│
├── sketch_jun20a              // Arduino Sketch for RFID + LED
│
├── appsettings.json           // Configuration (DB, Serial Port)
├── ArduinoAttendance.http     // HTTP request test file
├── Program.cs                 // DI, Middleware setup
└── README.md                  // This file
```

---

## 🔌 Arduino Uno Wiring

| MFRC522 Pin | Arduino Uno Pin |
|-------------|------------------|
| **SDA**    | 10               |
| **SCK**    | 13               |
| **MOSI**   | 11               |
| **MISO**   | 12               |
| **RST**    | 9                |
| **3.3V**   | 3.3V             |
| **GND**    | GND              |

#### LED Indicators:
| Function      | Pin |
|--------------|-----|
| **Green LED** (Success) | 6   |
| **Red LED** (Fail)      | 7   |

---

## 🚀 System Flow

1. 👤 **User taps RFID card on MFRC522 (Arduino Uno)**.
2. 📡 Arduino reads the tag and sends the RFID string to the backend via **Serial Port COM5**.
3. 🖥️ **.NET Core Backend:**
   - First Scan (today): **Login** record saved (`SignedStatus.Login`).
   - Second Scan (today): **Logout** record saved (`SignedStatus.Logout`).
   - Further Scans (today): **Ignored**, no duplicate entries allowed.
4. ✅ Success response (`LOGIN_OK`, `LOGOUT_OK`) sent back to Arduino → Green LED blinks.
5. ❌ Fail/Unknown card → Red LED blinks.

---

## 📝 Attendance Rules

| Scenario                     | System Response | LED  |
|-----------------------------|----------------|------|
| **First scan of the day**    | Login          | Green|
| **Second scan (logout)**     | Logout         | Green|
| **More than two scans/day**  | Ignored        | Red  |
| **Unknown RFID**             | Fail           | Red  |

---

## ⚠️ Important Notes

- ✅ **No multiple Login/Logout per day per user**.
- ⚠️ Ensure **COM Port (COM5)** is NOT in use by Arduino IDE or other applications when running `.NET Core`.
- ⏳ Database handles timestamps (`CreatedAt`, `LastModified`) **automatically** — Frontend cannot set these values.

---

## 🏁 Running the System

1. Flash Arduino Uno with the code from `sketch_jun20a`.
2. Connect Arduino to PC (**check COM5**).
3. Run `.NET Core` backend:
   ```
   dotnet run --project ArduinoAttendance
   ```
4. Open Swagger (`https://localhost:7156/swagger`) to manage Users/Attendances.
5. Tap RFID cards to mark attendance.

---

## 📦 Future Features (Optional)

- 🔍 **Admin Dashboard** (Angular/React)
- 📸 Capture photo during scan.
- ✉️ Email/Push notifications.
- 🕑 Shift/Working hour reports.

---

## 👤 Author

**Ishan Maduwantha**  
_Software Engineer — Sri Lanka_

---

> This is an academic/learning project combining hardware + software for modern RFID-based attendance tracking.
