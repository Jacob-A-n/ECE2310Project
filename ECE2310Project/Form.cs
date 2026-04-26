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
        private int year;
        private int month;
        private Label[] labelDays;
        private Label[] labelEvent;
        private Calendar Cal = CultureInfo.InvariantCulture.Calendar;
        private List<CalendarEvent> events = new List<CalendarEvent>();
        private List<RecurringEvent> recurringEvents = new List<RecurringEvent>();
        private DrawCalendar drawCalendar = new DrawCalendar();

        public Form()
        {
            InitializeComponent();

            labelDays = new Label[] { labelDay0, labelDay1, labelDay2, labelDay3, labelDay4, labelDay5, labelDay6, labelDay7, labelDay8, labelDay9, labelDay10, labelDay11, labelDay12, labelDay13, labelDay14, labelDay15, labelDay16, labelDay17, labelDay18, labelDay19, labelDay20, labelDay21, labelDay22, labelDay23, labelDay24, labelDay25, labelDay26, labelDay27, labelDay28, labelDay29, labelDay30, labelDay31, labelDay32, labelDay33, labelDay34, labelDay35, labelDay36, labelDay37, labelDay38, labelDay39, labelDay40, labelDay41 };
            labelEvent = new Label[] { labelEvent0, labelEvent1, labelEvent2, labelEvent3, labelEvent4, labelEvent5, labelEvent6, labelEvent7, labelEvent8, labelEvent9, labelEvent10, labelEvent11, labelEvent12, labelEvent13, labelEvent14, labelEvent15, labelEvent16, labelEvent17, labelEvent18, labelEvent19, labelEvent20, labelEvent21, labelEvent22, labelEvent23, labelEvent24, labelEvent25, labelEvent26, labelEvent27, labelEvent28, labelEvent29, labelEvent30, labelEvent31, labelEvent32, labelEvent33, labelEvent34, labelEvent35, labelEvent36, labelEvent37, labelEvent38, labelEvent39, labelEvent40, labelEvent41 };
            year = drawCalendar.DateWindow.Year;
            month = drawCalendar.DateWindow.Month;

            //some national holidays
            recurringEvents.Add(new RecurringEvent("New Year's Day", year, 1, 1));
            recurringEvents.Add(new RecurringEvent("New Year's Eve", year, 12, 31));
            recurringEvents.Add(new RecurringEvent("Independence Day", year, 7, 4));
            recurringEvents.Add(new RecurringEvent("Christmas", year, 12, 25));
            recurringEvents.Add(new RecurringEvent("Halloween", year, 10, 31));
            recurringEvents.Add(new RecurringEvent("Valentine's Day", year, 2, 14));
            recurringEvents.Add(new RecurringEvent("Juneteenth", year, 6, 19));

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
                //add label event reset later

                //text clarity
                if (dCal.isDayOfMonth[i] || !checkBoxHideDays.Checked)
                {
                    labelDays[i].ForeColor = Color.Black;
                }
                else
                {
                    labelDays[i].ForeColor = Color.LightGray;
                }

                if (dCal.isDayOfMonth[i] || !checkBoxHideEvents.Checked)
                {
                    labelEvent[i].ForeColor = Color.Black;
                }
                else
                {
                    labelEvent[i].ForeColor = Color.LightGray;
                }

                if (dCal.DateWindow.Month == DateTime.Now.Month && dCal.DateWindow.Year == DateTime.Now.Year)
                {
                    if (dCal.isDayOfMonth[i] && DateTime.Now.Day == int.Parse(labelDays[i].Text))
                    {
                        labelDays[i].ForeColor = Color.Red;
                    }
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

            //local method
            void DrawEvents(int eventIndex, int calendarIndex, string type)
            {
                if (type == "recurring")
                {
                    labelEvent[calendarIndex].Text += recurringEvents[eventIndex].Name + "\n";
                }
                else if (type == "event")
                {
                    labelEvent[calendarIndex].Text += events[eventIndex].Name + "\n";
                }
                //add more later for different type of events
            }

            //finds the placement of recurring events on the calendar
            for (int i = 0; i < recurringEvents.Count; i++) //for each event
            {
                int day = recurringEvents[i].GetDayOfMonth();
                for (int j = 0; j < 42; j++) //for each calendar day on screen
                {
                    int eMonth = recurringEvents[i].DateInfo.Month;

                    //viewing janurary case
                    if (month == 1)
                    {
                        if (eMonth == 12)
                        {
                            if (j < dCal.StartOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j, "recurring");
                            }
                        }
                        else if (eMonth == 1)
                        {
                            if (j >= dCal.StartOfMonth && j < dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j, "recurring");
                            }
                        }
                        else if (eMonth == 2)
                        {
                            if (j >= dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j, "recurring");
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
                                DrawEvents(i, j, "recurring");
                            }
                        }
                        else if (eMonth == 12)
                        {
                            if (j >= dCal.StartOfMonth && j < dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j, "recurring");
                            }
                        }
                        else if (eMonth == 1)
                        {
                            if (j >= dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j, "recurring");
                            }
                        }
                    }

                    //general case
                    else if (eMonth == month - 1)
                    {
                        if (j < dCal.StartOfMonth && labelDays[j].Text == day.ToString())
                        {
                            DrawEvents(i, j, "recurring");
                        }
                    }
                    else if (eMonth == month)
                    {
                        if (j >= dCal.StartOfMonth && j < dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                        {
                            DrawEvents(i, j, "recurring");
                        }
                    }
                    else if (eMonth == month + 1)
                    {
                        if (j >= dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                        {
                            DrawEvents(i, j, "recurring");
                        }
                    }
                }
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
                        if (eMonth == 12 && events[i].DateInfo.Year == dCal.DateWindow.Year - 1)
                        {
                            if (j < dCal.StartOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j, "event");
                            }
                        }
                        else if (eMonth == 1 && events[i].DateInfo.Year == dCal.DateWindow.Year)
                        {
                            if (j >= dCal.StartOfMonth && j < dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j, "event");
                            }
                        }
                        else if (eMonth == 2 && events[i].DateInfo.Year == dCal.DateWindow.Year)
                        {
                            if (j >= dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j, "event");
                            }
                        }
                    }

                    //december case
                    else if (month == 12)
                    {
                        if (eMonth == 11 && events[i].DateInfo.Year == dCal.DateWindow.Year)
                        {
                            if (j < dCal.StartOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j, "event");
                            }
                        }
                        else if (eMonth == 12 && events[i].DateInfo.Year == dCal.DateWindow.Year)
                        {
                            if (j >= dCal.StartOfMonth && j < dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j, "event");
                            }
                        }
                        else if (eMonth == 1 && events[i].DateInfo.Year == dCal.DateWindow.Year + 1)
                        {
                            if (j >= dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                            {
                                DrawEvents(i, j, "event");
                            }
                        }
                    }

                    //general case
                    else if (eMonth == month - 1 && events[i].DateInfo.Year == dCal.DateWindow.Year)
                    {
                        if (j < dCal.StartOfMonth && labelDays[j].Text == day.ToString())
                        {
                            DrawEvents(i, j, "event");
                        }
                    }
                    else if (eMonth == month && events[i].DateInfo.Year == dCal.DateWindow.Year)
                    {
                        if (j >= dCal.StartOfMonth && j < dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                        {
                            DrawEvents(i, j, "event");
                        }
                    }
                    else if (eMonth == month + 1 && events[i].DateInfo.Year == dCal.DateWindow.Year)
                    {
                        if (j >= dCal.EndOfMonth && labelDays[j].Text == day.ToString())
                        {
                            DrawEvents(i, j, "event");
                        }
                    }
                }
            }
        }

        private void Notification()
        {
            for (int i = events.Count - 1; i >= 0; i--)
            {
                if (DateTime.Compare(events[i].DateInfo, DateTime.Now) <= 0 && !events[i].NotificationSent)
                {
                    if (events[i].Discription == "")
                    {
                        events[i].NotificationSent = true;
                        MessageBox.Show("Event: " + events[i].Name , "Event Notification");
                    }
                    else
                    {
                        events[i].NotificationSent = true;
                        MessageBox.Show("Event: " + events[i].Name + "\nDescription: " + events[i].Discription, "Event Notification");
                    }
                    if (checkBoxDeleteEventNow.Checked)
                    {
                        events.RemoveAt(i);
                    }
                }
                else if (DateTime.Compare(events[i].DateInfo, Cal.AddDays(DateTime.Now, -5)) <= 0) //removes events that are 5 days or more older
                {
                    events.RemoveAt(i);
                }
            }

            for (int i = recurringEvents.Count - 1; i >= 7; i--) //fix
            {
                if (DateTime.Compare(recurringEvents[i].DateInfo, DateTime.Now) <= 0)
                {
                    if (recurringEvents[i].Discription == "")
                    {
                        recurringEvents[i].UpdateNotificationTime();
                        MessageBox.Show("Event: " + recurringEvents[i].Name, "Event Notification");
                    }
                    else
                    {
                        recurringEvents[i].UpdateNotificationTime();
                        MessageBox.Show("Event: " + recurringEvents[i].Name + "\nDescription: " + recurringEvents[i].Discription, "Event Notification");
                    }
                }
            }
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
            if (checkBoxRecurringEvent.Checked)
            {
                recurringEvents.Add(new RecurringEvent(textBoxEventName.Text, dateTimePickerEventDate.Value.Year, dateTimePickerEventDate.Value.Month, dateTimePickerEventDate.Value.Day, (int)numericUpDownTimeHour.Value, (int)numericUpDownTimeMinute.Value, textBoxEventDecription.Text));
            }
            else
            {
                events.Add(new CalendarEvent(textBoxEventName.Text, dateTimePickerEventDate.Value.Year, dateTimePickerEventDate.Value.Month, dateTimePickerEventDate.Value.Day, (int)numericUpDownTimeHour.Value, (int)numericUpDownTimeMinute.Value, textBoxEventDecription.Text));
            }
            BuildCalendar(drawCalendar);
            buttonCreateEvent.Enabled = false;
            textBoxEventName.Text = "";
            textBoxEventDecription.Text = "";
            checkBoxRecurringEvent.Checked = false;
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

        private void checkBoxHideDays_CheckedChanged(object sender, EventArgs e)
        {
            BuildCalendar(drawCalendar);
        }

        private void checkBoxHideEvents_CheckedChanged(object sender, EventArgs e)
        {
            BuildCalendar(drawCalendar);
        }

        private void buttonGoTo_Click(object sender, EventArgs e)
        {
            drawCalendar = new DrawCalendar(dateTimePickerGoTo.Value.Year, dateTimePickerGoTo.Value.Month);
            year = drawCalendar.DateWindow.Year;
            month = drawCalendar.DateWindow.Month;
            Console.WriteLine(year + " " + month);
            BuildCalendar(drawCalendar);
            Console.WriteLine(drawCalendar.ToString());
        }

        private void timerCalendar_Tick(object sender, EventArgs e)
        {
            Notification();
            BuildCalendar(drawCalendar);
        }
    }
}
