using System;
using System.Windows.Forms;

namespace ECE2310Project
{
    public class Project
    {
        static void Main()
        {
            //Initalizes GUI
            Form windowForm = new Form();
            windowForm.ShowDialog();

            //debugging and testing below
            // Go to Form.cs to write code that will run when the form is loaded.
            CalendarEvent ev = new CalendarEvent("name",2026, 4, 16, 10, 0); 
            Console.WriteLine(ev.GetDayOfWeek() + "\n\n");

            DrawCalendar drawCalendar = new DrawCalendar(2026,4);
            Console.WriteLine(drawCalendar + "\n\n");

            DrawCalendar drawCalendar1 = new DrawCalendar(2026, 2);
            Console.WriteLine(drawCalendar1);

            Console.WriteLine();
        }
    }
}

