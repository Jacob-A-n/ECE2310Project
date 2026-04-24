using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECE2310Project
{
    public class DrawCalendar
    //This class will manage the GUI for the Calendar
    {
        public DateTime DateWindow { get; set; }
        private Calendar Cal = CultureInfo.InvariantCulture.Calendar;
        private int year;
        private int month;
        public int[] calendarNumbers = new int[42];
        public bool[] isDayOfMonth = new bool[42];
        public int StartOfMonth { get; set; }
        public int EndOfMonth { get; set; }

        public DrawCalendar(int year, int month)
        {
            DateWindow = new DateTime(year, month, 1, 0, 0, 0, new GregorianCalendar());
            this.year = year;
            this.month = month;
        }
        
        public DrawCalendar(DateTime dateWindow)
        {
            DateWindow = dateWindow;
        }

        public DrawCalendar() //BROKEN DONT USE // SETS TIME TO YEAR 0 // CAUSES ERRORS
        {
            DateWindow = DateTime.Now;
        }

        public void Update(int year, int month)
        {
            DateWindow = new DateTime(year, month, 1, 0, 0, 0, new GregorianCalendar());
            this.year = year;
            this.month = month;
        }

        //updates list to use to display dates
        public void ArrangeCalendar(DayOfWeek day)
        {
            StartOfMonth = 0;
            EndOfMonth = 0;
            for (int i = 0; i < isDayOfMonth.Length; i++)
            {
                isDayOfMonth[i] = false;
            }

            switch (day.ToString()) //finds which day is start of the month
            {
                case ("Sunday"): StartOfMonth = 0; break;
                case ("Monday"): StartOfMonth = 1; break;
                case ("Tuesday"): StartOfMonth = 2; break;
                case ("Wednesday"):  StartOfMonth = 3; break;
                case ("Thursday"): StartOfMonth = 4; break;
                case ("Friday"): StartOfMonth = 5; break;
                case ("Saturday"): StartOfMonth = 6; break;
            }

            EndOfMonth = StartOfMonth + Cal.GetDaysInMonth(year, month);

            DateWindow = Cal.AddDays(DateWindow, -1); //gets last month //cannot use Cal.GetDaysInMonth() directly incase month was janurary; causes error
            for (int i = Cal.GetDaysInMonth(DateWindow.Year, DateWindow.Month), j = StartOfMonth - 1; i > Cal.GetDaysInMonth(DateWindow.Year, DateWindow.Month) - StartOfMonth; i--, j--)
            {
                calendarNumbers[j] = i;
            }
            DateWindow = Cal.AddDays(DateWindow, +1); //back to orginal date

            for (int i = 0; i < Cal.GetDaysInMonth(DateWindow.Year, DateWindow.Month); i++)
            {
                calendarNumbers[StartOfMonth + i] = i + 1;
                isDayOfMonth[StartOfMonth + i] = true;
            }

            for (int i = EndOfMonth, j = 1 ; i < calendarNumbers.Length; i++, j++)
            {
                calendarNumbers[i] = j;
            }
        }

        public override string ToString()
        {
            ArrangeCalendar(Cal.GetDayOfWeek(DateWindow));
            string month = "";

            switch (Cal.GetMonth(DateWindow))
            {
                case (1): month = "January"; break;
                case (2): month = "February"; break;
                case (3): month = "March"; break;
                case (4): month = "April"; break;
                case (5): month = "May"; break;
                case (6): month = "June"; break;
                case (7): month = "July"; break;
                case (8): month = "August"; break;
                case (9): month = "September"; break;
                case (10): month = "October"; break;
                case (11): month = "November"; break;
                case (12): month = "December"; break;
            }

            string output = $"{month} {year}\nS  M  T  W  T  F  S\n";
            for (int i = 0; i < calendarNumbers.Length; i++) 
            {
                if (calendarNumbers[i] > 9)
                {
                    output += $"{calendarNumbers[i]} ";
                }
                else
                {
                    output += $"{calendarNumbers[i]}  ";
                }
                if (((i+1)%7) == 0)
                {
                    output += "\n";
                }
            }
            return output;
        }
    }
}
