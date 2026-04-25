using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECE2310Project
{
    public partial class Form : System.Windows.Forms.Form
    {
        private int year = 2026;
        private int month = 4;
        private Label[] labelDays;
        private Label[] labelEvent;
        private Calendar Cal = CultureInfo.InvariantCulture.Calendar;
        private List<CalendarEvent> events = new List<CalendarEvent>();
        private DrawCalendar drawCalendar = new DrawCalendar(2026,4);

        public Form()
        {
            InitializeComponent();

            labelDays = new Label[] { labelDay0, labelDay1, labelDay2, labelDay3, labelDay4, labelDay5, labelDay6, labelDay7, labelDay8, labelDay9, labelDay10, labelDay11, labelDay12, labelDay13, labelDay14, labelDay15, labelDay16, labelDay17, labelDay18, labelDay19, labelDay20, labelDay21, labelDay22, labelDay23, labelDay24, labelDay25, labelDay26, labelDay27, labelDay28, labelDay29, labelDay30, labelDay31, labelDay32, labelDay33, labelDay34, labelDay35, labelDay36, labelDay37, labelDay38, labelDay39, labelDay40, labelDay41 };
            labelEvent = new Label[] { labelEvent0, labelEvent1, labelEvent2, labelEvent3, labelEvent4, labelEvent5, labelEvent6, labelEvent7, labelEvent8, labelEvent9, labelEvent10, labelEvent11, labelEvent12, labelEvent13, labelEvent14, labelEvent15, labelEvent16, labelEvent17, labelEvent18, labelEvent19, labelEvent20, labelEvent21, labelEvent22, labelEvent23, labelEvent24, labelEvent25, labelEvent26, labelEvent27, labelEvent28, labelEvent29, labelEvent30, labelEvent31, labelEvent32, labelEvent33, labelEvent34, labelEvent35, labelEvent36, labelEvent37, labelEvent38, labelEvent39, labelEvent40, labelEvent41 };

  
            BuildCalendar(drawCalendar);
        }

        public void BuildCalendar(DrawCalendar dCal)
        {
            //month number text
            dCal.ArrangeCalendar(dCal.DateWindow.DayOfWeek);
            for (int i = 0; i < 42; i++) 
            {
                labelDays[i].Text = dCal.calendarNumbers[i].ToString();
                labelEvent[i].Text = "";

                //text clarity
                if (dCal.isDayOfMonth[i])
                {
                    labelDays[i].ForeColor = Color.Black;
                }
                else
                {
                    labelDays[i].ForeColor = Color.LightGray;
                }
            }

            switch (month)
            {
                case (1): textBoxMonth.Text = "January"; break;
                case (2): textBoxMonth.Text = "February"; break;
                case (3): textBoxMonth.Text = "March"; break;
                case (4): textBoxMonth.Text = "April"; break;
                case (5): textBoxMonth.Text = "May"; break;
                case (6): textBoxMonth.Text = "June"; break;
                case (7): textBoxMonth.Text = "July"; break;
                case (8): textBoxMonth.Text = "August"; break;
                case (9): textBoxMonth.Text = "September"; break;
                case (10): textBoxMonth.Text = "October"; break;
                case (11): textBoxMonth.Text = "November"; break;
                case (12): textBoxMonth.Text = "December"; break;
            }
            textBoxMonth.Text = textBoxMonth.Text + ", " + year.ToString();

            void DrawEvents(int eventIndex, int calendarIndex)
            {
                labelEvent[calendarIndex].Text += events[eventIndex].Name + "\n";
                //add more later for different type of events
            }

                //finds the placement of events on the calendar
            for (int i = 0; i < events.Count; i++) //for each event
            {
                int day = events[i].GetDayOfMonth();
                for (int j = 0; j < 42; j++) //for each calendar day on screen
                {
                    int eMonth = events[i].DateInfo.Month;

                    //viewing janurary case
                    if (month == 1)
                    {
                        if (eMonth == 12)
                        {
                            if (j < dCal.StartOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j);
                            }
                        }
                        else if (eMonth == 1)
                        {
                            if (j >= dCal.StartOfMonth && j < dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j);
                            }
                        }
                        else if (eMonth == 2)
                        {
                            if (j >= dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j);
                            }
                        }
                    }

                    //december case
                    else if (month == 12)
                    {
                        if (eMonth == 11)
                        {
                            if (j < dCal.StartOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j);
                            }
                        }
                        else if (eMonth == 12)
                        {
                            if (j >= dCal.StartOfMonth && j < dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j);
                            }
                        }
                        else if (eMonth == 1)
                        {
                            if (j >= dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j);
                            }
                        }
                    }

                    //general case
                    else if (eMonth == month - 1)
                    {
                        if (j < dCal.StartOfMonth && labelDays[j].Text == day.ToString())
                        {
                            DrawEvents(i, j);
                        }
                    }
                    else if (eMonth == month)
                    {
                        if (j >= dCal.StartOfMonth && j < dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                        {
                            DrawEvents(i, j);
                        }
                    }
                    else if (eMonth == month + 1)
                    {
                        if (j >= dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                        {
                            DrawEvents(i, j);
                        }
                    }
                }
            }

            //more features later
        }


        //ALL EVENT TRIGGERS BELOW
        private void buttonRedcedeMonth_Click(object sender, EventArgs e)
        {
            if (month == 1)
            {
                month = 12;
                year--;
            }
            else
            {
                month--;
            }
            dateTimePickerEventDate.Value = new DateTime(year, month, 1);
            drawCalendar.SubtractMonth();
            BuildCalendar(drawCalendar);
            Console.WriteLine(drawCalendar.ToString());
        }

        private void buttonAdvanceMonth_Click(object sender, EventArgs e)
        {
            if (month == 12)
            {
                month = 1;
                year++;
            }
            else
            {
                month++;
            }
            drawCalendar.AddMonth();
            BuildCalendar(drawCalendar);
            Console.WriteLine(drawCalendar.ToString());
        }

        private void buttonCreateEvent_Click(object sender, EventArgs e)
        {
            events.Add(new CalendarEvent(textBoxEventName.Text, dateTimePickerEventDate.Value.Year, dateTimePickerEventDate.Value.Month, dateTimePickerEventDate.Value.Day, (int)numericUpDownTimeHour.Value, (int)numericUpDownTimeMinute.Value));
            BuildCalendar(drawCalendar);
            buttonCreateEvent.Enabled = false;
            textBoxEventName.Text = "";
            textBoxEventDecription.Text = "";
        }

        private void textBoxEventName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxEventName.Text))
            {
                buttonCreateEvent.Enabled = true;
            }
            else
            {
                buttonCreateEvent.Enabled = false;
            }
        }
    }
}
