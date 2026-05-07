# 5. Implementation Details

## 5.1 Languages, Frameworks, and Core Dependencies
- **Language:** C# (project targets .NET Framework 4.7.2) based on the project file configuration.  
- **UI Framework:** Windows Forms for the desktop GUI and event-driven interactions.  
- **Core .NET libraries used:**  
  - `System.Windows.Forms` for UI controls and dialogs (Windows Forms)  
  - `System.Drawing` for UI styling and color usage  
  - `System.Globalization` for calendar and date calculations (Calendar/DateTime usage)  

References:  
- Windows Forms overview: https://learn.microsoft.com/en-us/dotnet/desktop/winforms/  
- .NET Framework overview: https://learn.microsoft.com/en-us/dotnet/framework/  
- DateTime API: https://learn.microsoft.com/en-us/dotnet/api/system.datetime  

## 5.2 Third-Party Libraries
This project does **not** use any third-party packages or NuGet dependencies; it relies exclusively on the .NET Framework libraries listed above (see Windows Forms and DateTime references).

## 5.3 Build and Run Instructions
From the repository root (Windows PowerShell):
1. Build:  
   `dotnet build "ECE2310Project/ECE2310Project.csproj"`
2. Run the executable:  
   `& "ECE2310Project/bin/Debug/ECE2310Project.exe"`

> Note: The project targets .NET Framework 4.7.2, which typically requires the Windows targeting pack on the build machine.

## 5.4 Module Breakdown
- **Project.cs**: Application entry point; launches the main `Form` (Windows Form).  
- **Form.cs / Form.Designer.cs**: Primary UI, calendar rendering, event creation, notifications, focus timer, stats, and notes tabs.  
- **DrawCalendar.cs**: Calendar grid generation and month navigation logic.  
- **Event.cs**: Domain models for `CalendarEvent`, `RecurringEvent`, and `StudentNote`.  

## 5.5 Notable Algorithms & Data Structures (brief)
- **Calendar grid generation (DrawCalendar.ArrangeCalendar)**  
  - Uses fixed-size arrays of length **42** (6 weeks x 7 days) to render any month view.  
  - Complexity: **O(42)** for array setup and fill, plus constant-time calendar lookups.  
- **Event placement on calendar (Form.BuildCalendar)**  
  - For each event/recurring event, scans the 42-day grid to place labels.  
  - Complexity: **O(42 × (E + R))**, where *E* = one-time events and *R* = recurring events.  
- **Duplicate event detection (AddEvent)**  
  - Linear scan of `eventTracker` list.  
  - Complexity: **O(N)**.  
- **Notification sweep (Notification)**  
  - Linear scans through event lists to trigger alerts and cleanup.  
  - Complexity: **O(E + R)** per timer tick.  

# 9. Appendices

## A. Setup Instructions (also in README)
1. Install .NET Framework 4.7.2 targeting pack (required to build).  
2. Follow the build/run steps in **Section 5.3**.

## B. API Documentation
No generated API documentation is included in this repository. Public APIs are minimal and primarily internal classes for UI/event management.

## C. Additional Diagrams
No additional diagrams are included. The UI layout is defined programmatically in `Form.cs` and by the Windows Forms designer (`Form.Designer.cs`).

## D. Sample Data / Defaults
- **Built-in recurring holidays** (configured at startup in `Form`):  
  - New Year’s Day, New Year’s Eve, Independence Day, Christmas, Halloween, Valentine’s Day, Juneteenth.  
- **Project work log**: See `ECE2310Project/ProjectLog.txt` for historical updates.  
- **Calendar output sample**: `DrawCalendar.ToString()` produces a text calendar grid useful for debugging.
