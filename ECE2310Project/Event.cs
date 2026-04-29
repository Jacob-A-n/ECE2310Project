using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECE2310Project
{
    public enum RecurrenceFrequency
    {
        Once = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4
    }

    public class CalendarEvent //base class for Events
    {
        public bool NotificationSent = false;
        public DateTime DateInfo { get; set; }//  https://learn.microsoft.com/en-us/dotnet/api/system.datetime?view=net-10.0 regarding DateTime class
        // public DateTime(int year, int month, int day, int hour, int minute, int second, System.Globalization.Calendar calendar); // constructor for DateTime
        protected Calendar Cal = CultureInfo.InvariantCulture.Calendar;
        public string Name { get; set; }
        public string Discription { get; set; }
        public bool IsFinished { get; set; }
        public string Note { get; set; }

        public CalendarEvent(string name, int year, int month, int day, int hour = 0, int minute = 0, string discription = "")
        {
            try
            {
                DateInfo = new DateTime(year, month, day, hour, minute, 0, new GregorianCalendar());
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Out of range argument for CalendarEvent");
            }
            Name = name;
            Discription = discription;
            IsFinished = false;
            Note = "";
        }

        public int GetDayOfMonth() => Cal.GetDayOfMonth(DateInfo);
        public string GetDayOfWeek() => Cal.GetDayOfWeek(DateInfo).ToString();
        public int GetDatOfYear() => Cal.GetDayOfYear(DateInfo);
        public int GetDaysInMonth() => Cal.GetDaysInMonth(DateInfo.Year,DateInfo.Month);
        public int GetDaysInYear() => Cal.GetDaysInYear(DateInfo.Year);
        public int GetLeapMonth() => Cal.GetLeapMonth(DateInfo.Year); //might delete later
        public bool IsLeapDay() => Cal.IsLeapDay(DateInfo.Year, DateInfo.Month, DateInfo.Day);
        public bool IsLeapMonth() => Cal.IsLeapMonth(DateInfo.Year, DateInfo.Month);
        public bool IsLeapYear() => Cal.IsLeapYear(DateInfo.Year);
    }

    // Fix: ensure recurring constructor advances to next year when the date is in the past
    public class RecurringEvent : CalendarEvent
    {
        // store recurrence frequency so UI and editors can show correct selection
        public RecurrenceFrequency Frequency { get; set; } = RecurrenceFrequency.Yearly;

        // constructor with optional frequency (defaults to Yearly so existing behavior preserved)
        public RecurringEvent(string name, int year, int month, int day, int hour = 0, int minute = 0, string discription = "", RecurrenceFrequency frequency = RecurrenceFrequency.Yearly) 
            : base(name, year, month, day, hour, minute, discription)
        {
            Frequency = frequency;

            // if the provided date is in the past, advance to next occurrence (yearly behavior preserved)
            if (DateTime.Compare(DateInfo, DateTime.Now) < 0)
            {
                UpdateNotificationTime(year);
            }
        }

        public void UpdateNotificationTime()
        {
            // currently only advances a year (keeps prior behavior).
            // future: adjust according to Frequency (Daily/Weekly/Monthly) if needed.
            if (DateTime.IsLeapYear(DateInfo.Year + 1) && DateInfo.Month == 2 && DateInfo.Day == 29)
            {
                DateInfo = new DateTime(DateInfo.Year + 1, DateInfo.Month, DateInfo.Day - 1, DateInfo.Hour, DateInfo.Minute, 0, new GregorianCalendar());
            }
            else //all other cases
            {
                DateInfo = new DateTime(DateInfo.Year + 1, DateInfo.Month, DateInfo.Day, DateInfo.Hour, DateInfo.Minute, 0, new GregorianCalendar());
            }
        }

        public void UpdateNotificationTime(int year)
        {
            if (DateTime.IsLeapYear(year) && DateInfo.Month == 2 && DateInfo.Day == 29)
            {
                DateInfo = new DateTime(year + 1, DateInfo.Month, DateInfo.Day - 1, DateInfo.Hour, DateInfo.Minute, 0, new GregorianCalendar());
            }
            else //all other cases
            {
                DateInfo = new DateTime(year + 1, DateInfo.Month, DateInfo.Day, DateInfo.Hour, DateInfo.Minute, 0, new GregorianCalendar());
            }
        }
    }

    public class StudentNote
    {
        public string Title { get; set; }
        public string Words { get; set; }
        public string AttachedEvent { get; set; }

        public StudentNote(string title, string words, string attachedEvent)
        {
            Title = title;
            Words = words;
            AttachedEvent = attachedEvent;
        }

        public override string ToString()
        {
            if (AttachedEvent == "")
            {
                return Title;
            }
            return Title + " (" + AttachedEvent + ")";
        }
    }
}
