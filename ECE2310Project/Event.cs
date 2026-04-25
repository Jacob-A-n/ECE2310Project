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
    public class CalendarEvent //base class for Events
    {
        public DateTime DateInfo { get; set; }//  https://learn.microsoft.com/en-us/dotnet/api/system.datetime?view=net-10.0 regarding DateTime class
        // public DateTime(int year, int month, int day, int hour, int minute, int second, System.Globalization.Calendar calendar); // constructor for DateTime
        private Calendar Cal = CultureInfo.InvariantCulture.Calendar;
        public string Name { get; set; }
        public string Discription { get; set; }

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
}
