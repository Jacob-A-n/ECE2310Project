# ECE2310 Project

A C# Windows Event Planner application.

## Authors

### Ari Akram, Jacob An, Jacob Medel, Areg Hovumyan

## Features
* Interactive calendar visualization via standard WinForms
* Event creation and management system
* Date-aware math via `CultureInfo.InvariantCulture.Calendar`
* Tracking for active/completed events and recurring schedules
* Student note feature attached to calendar events
* Simple console reporting (in `Project.cs`) for debugging
* Object-oriented code design in C#

## Code Structure & Functionality

### Root Files
* `ECE2310Project.sln`: Visual Studio Solution file for managing the project.
* `ECE2310Project.slnx`: Modern XML-based Visual Studio Solution file.
* `clean-and-run.ps1`: PowerShell automation script to quickly build and execute the application.
* `README.md`: This documentation file.

### Application Files (`ECE2310Project/`)
* `Event.cs`: Defines core event data models
  * `CalendarEvent`: Base model featuring properties like Title, Description, DateInfo, Date Mathematics, and Completion Flags.
  * `RecurringEvent`: Handles events occurring periodically and auto-advances to the next occurrence (taking leap years into account).
  * `StudentNote`: Manages arbitrary text annotations mapped to independent events.
* `DrawCalendar.cs`: Contains the calendar generation and rendering logic
  * Populates arrays based on `DateTime` for month generation.
  * Includes functions for padding months (figuring out days before/after current month grid).
  * Automatically calculates leap years and varied month durations.
* `Project.cs`: Bootstraps the application, calling `Form.ShowDialog()` and executing debug traces.
* `Form.cs`: Core logic powering the graphical user interface.

## Prerequisites
* .NET SDK
* Windows Operating System

## Build and Execute
* Open PowerShell or Command Prompt
* Build using the CLI:
  * `dotnet build "ECE2310Project\ECE2310Project.csproj"`
* Run the application: 
  * `& "ECE2310Project\bin\Debug\ECE2310Project.exe"`
* Or use the provided script:
  * `.\clean-and-run.ps1`
