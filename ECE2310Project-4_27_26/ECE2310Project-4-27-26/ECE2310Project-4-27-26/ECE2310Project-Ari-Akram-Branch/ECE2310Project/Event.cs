using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;

namespace ECE2310Project
{
    public class CalendarEvent //base class for Events
    {
        public bool NotificationSent = false;
        public DateTime DateInfo { get; set; } // date+time of the original occurrence
        protected Calendar Cal = CultureInfo.InvariantCulture.Calendar;
        public string Name { get; set; }
        public string Discription { get; set; }
        public bool IsFinished { get; set; }
        public string Note { get; set; }

        // Recurrence fields
        public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.Once;
        public int RecurrenceInterval { get; set; } = 1; // interval for recurrence (every N units)
        public DateTime? RecurrenceEnd { get; set; } = null; // optional end date for the series

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
            RecurrenceType = RecurrenceType.Once;
            RecurrenceInterval = 1;
            RecurrenceEnd = null;
        }

        public int GetDayOfMonth() => Cal.GetDayOfMonth(DateInfo);
        public string GetDayOfWeek() => Cal.GetDayOfWeek(DateInfo).ToString();
        public int GetDatOfYear() => Cal.GetDayOfYear(DateInfo);
        public int GetDaysInMonth() => Cal.GetDaysInMonth(DateInfo.Year, DateInfo.Month);
        public int GetDaysInYear() => Cal.GetDaysInYear(DateInfo.Year);
        public int GetLeapMonth() => Cal.GetLeapMonth(DateInfo.Year); //might delete later
        public bool IsLeapDay() => Cal.IsLeapDay(DateInfo.Year, DateInfo.Month, DateInfo.Day);
        public bool IsLeapMonth() => Cal.IsLeapMonth(DateInfo.Year, DateInfo.Month);
        public bool IsLeapYear() => Cal.IsLeapYear(DateInfo.Year);
    }

    public class RecurringEvent : CalendarEvent
    {
        // Tracks the last occurrence (date/time) that was notified, prevents duplicate notifications for the same occurrence.
        public DateTime? LastNotifiedOccurrence { get; set; } = null;

        public RecurringEvent(string name, int year, int month, int day, int hour = 0, int minute = 0, string discription = "") : base(name, year, month, day, hour, minute, discription)
        {
            if (DateTime.Compare(DateInfo, DateTime.Now) < 0) //if event is in the past, updates to next year (legacy yearly behavior)
            {
                UpdateNotificationTime(year);
            }
        }

        // Legacy helper for yearly-only events (keeps previous behaviour)
        public void UpdateNotificationTime()
        {
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
