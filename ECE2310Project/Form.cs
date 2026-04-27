using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private List<CalendarEvent> eventTracker = new List<CalendarEvent>();
        private DrawCalendar drawCalendar = new DrawCalendar();
        private List<StudentNote> notes = new List<StudentNote>();
        private int totalFocusSeconds = 0;
        private int focusSecondsLeft = 0;
        private int focusSecondsPassed = 0;
        private int totalEventsMade = 0;
        private int totalEventsFinished = 0;
        private bool focusRunning = false;
        private bool usingStopWatch = false;
        private int editingEventIndex = -1;


        private TabPage tabPageFocus;
        private TabPage tabPageStats;
        private TabPage tabPageNotes;
        private Label labelFocusTime;
        private Label labelStats;
        private Label labelNoteBody;
        private Button buttonFocusStart;
        private Button buttonFocusStop;
        private Button buttonFinishEvent;
        private NumericUpDown numericFocusMinutes;
        private NumericUpDown numericGoalMinutes;
        private NumericUpDown numericGoalPercent;
        private RadioButton radioTimer;
        private RadioButton radioStopWatch;
        private ListBox listBoxNotes;
        private TextBox textBoxNoteTitle;
        private RichTextBox richTextBoxNote;
        private ComboBox comboBoxNoteEvents;
        private Timer timerFocus;

        public Form()
        {
            InitializeComponent();

            labelDays = new Label[] { labelDay0, labelDay1, labelDay2, labelDay3, labelDay4, labelDay5, labelDay6, labelDay7, labelDay8, labelDay9, labelDay10, labelDay11, labelDay12, labelDay13, labelDay14, labelDay15, labelDay16, labelDay17, labelDay18, labelDay19, labelDay20, labelDay21, labelDay22, labelDay23, labelDay24, labelDay25, labelDay26, labelDay27, labelDay28, labelDay29, labelDay30, labelDay31, labelDay32, labelDay33, labelDay34, labelDay35, labelDay36, labelDay37, labelDay38, labelDay39, labelDay40, labelDay41 };
            labelEvent = new Label[] { labelEvent0, labelEvent1, labelEvent2, labelEvent3, labelEvent4, labelEvent5, labelEvent6, labelEvent7, labelEvent8, labelEvent9, labelEvent10, labelEvent11, labelEvent12, labelEvent13, labelEvent14, labelEvent15, labelEvent16, labelEvent17, labelEvent18, labelEvent19, labelEvent20, labelEvent21, labelEvent22, labelEvent23, labelEvent24, labelEvent25, labelEvent26, labelEvent27, labelEvent28, labelEvent29, labelEvent30, labelEvent31, labelEvent32, labelEvent33, labelEvent34, labelEvent35, labelEvent36, labelEvent37, labelEvent38, labelEvent39, labelEvent40, labelEvent41 };
            year = drawCalendar.DateWindow.Year;
            month = drawCalendar.DateWindow.Month;
            numericUpDownTimeHour.Value = DateTime.Now.Hour;
            numericUpDownTimeMinute.Value = DateTime.Now.Minute;

            //some national holidays
            recurringEvents.Add(new RecurringEvent("New Year's Day", year, 1, 1));
            recurringEvents.Add(new RecurringEvent("New Year's Eve", year, 12, 31));
            recurringEvents.Add(new RecurringEvent("Independence Day", year, 7, 4));
            recurringEvents.Add(new RecurringEvent("Christmas", year, 12, 25));
            recurringEvents.Add(new RecurringEvent("Halloween", year, 10, 31));
            recurringEvents.Add(new RecurringEvent("Valentine's Day", year, 2, 14));
            recurringEvents.Add(new RecurringEvent("Juneteenth", year, 6, 19));

            BuildCalendar(drawCalendar);
            BuildExtraStudentFeatures();
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

        private void Notification() //also handles deleting events that are 5 or more days old
        {
            for (int i = events.Count - 1; i >= 0; i--)
            {
                if (DateTime.Compare(events[i].DateInfo, DateTime.Now) <= 0 && !events[i].NotificationSent)
                {
                    if (events[i].Discription == "")
                    {
                        events[i].NotificationSent = true;
                        MessageBox.Show("Event: " + events[i].Name, "Event Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        events[i].NotificationSent = true;
                        MessageBox.Show("Event: " + events[i].Name + "\nDescription: " + events[i].Discription, "Event Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    if (checkBoxDeleteEventNow.Checked)
                    {
                        for (int j = eventTracker.Count - 1; j >= 0; j--)
                        {
                            if (DateTime.Compare(eventTracker[j].DateInfo, events[i].DateInfo) == 0 && eventTracker[j].Name == events[i].Name)
                            {
                                eventTracker.RemoveAt(j);
                            }
                        }

                        events.RemoveAt(i);
                    }
                }
            }

            for (int i = recurringEvents.Count - 1; i >= 7; i--) //fix
            {
                if (DateTime.Compare(recurringEvents[i].DateInfo, DateTime.Now) <= 0)
                {
                    for (int j = eventTracker.Count - 1; j >= 0; j--)
                    {
                        if (DateTime.Compare(eventTracker[j].DateInfo, recurringEvents[i].DateInfo) == 0 && eventTracker[j].Name == recurringEvents[i].Name)
                        {
                            eventTracker[j].DateInfo = Cal.AddYears(eventTracker[j].DateInfo, 1);
                        }
                    }

                    if (recurringEvents[i].Discription == "")
                    {
                        recurringEvents[i].UpdateNotificationTime();
                        MessageBox.Show("Event: " + recurringEvents[i].Name, "Event Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        recurringEvents[i].UpdateNotificationTime();
                        MessageBox.Show("Event: " + recurringEvents[i].Name + "\nDescription: " + recurringEvents[i].Discription, "Event Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    UpdateList(); //updates times on the list
                }
            }
        }

        private void UpdateList()
        {
            listBoxEvents.Items.Clear();
            for (int i = 0; i < eventTracker.Count; i++)
            {
                listBoxEvents.Items.Add(eventTracker[i].Name + " on " + eventTracker[i].DateInfo.ToShortDateString());
            }

            UpdateEditEventList();
            UpdateNoteEventChoices();
        }

        private void AddEvent()
        {
            DateTime eventDateTime = new DateTime(
                dateTimePickerEventDate.Value.Year,
                dateTimePickerEventDate.Value.Month,
                dateTimePickerEventDate.Value.Day,
                (int)numericUpDownTimeHour.Value,
                (int)numericUpDownTimeMinute.Value,
                0);

            if (DateTime.Compare(eventDateTime, DateTime.Now) < 0 && !checkBoxMakePastEvents.Checked)
            {
                MessageBox.Show("The date of the event cannot be in the past. Please select a valid date or change the setting to allow past dates.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool copy = false;
            for (int i = 0; i < eventTracker.Count; i++)
            {
                if (dateTimePickerEventDate.Value.Year == eventTracker[i].DateInfo.Year && dateTimePickerEventDate.Value.Month == eventTracker[i].DateInfo.Month && dateTimePickerEventDate.Value.Day == eventTracker[i].DateInfo.Day && numericUpDownTimeHour.Value == eventTracker[i].DateInfo.Hour && numericUpDownTimeMinute.Value == eventTracker[i].DateInfo.Minute && eventTracker[i].Name == textBoxEventName.Text)
                {
                    copy = true;
                    break;
                }
            }

            if (copy)
            {
                MessageBox.Show("An event with the same name and date already exists. Please change the name or date of the event.", "Duplicate Event", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CalendarEvent trackedEvent = new CalendarEvent(
                textBoxEventName.Text,
                dateTimePickerEventDate.Value.Year,
                dateTimePickerEventDate.Value.Month,
                dateTimePickerEventDate.Value.Day,
                (int)numericUpDownTimeHour.Value,
                (int)numericUpDownTimeMinute.Value,
                textBoxEventDecription.Text);

            if (DateTime.Compare(eventDateTime, DateTime.Now) < 0)
            {
                trackedEvent.NotificationSent = true;
            }

            if (checkBoxRecurringEvent.Checked)
            {
                RecurringEvent recurringEvent = new RecurringEvent(
                    textBoxEventName.Text,
                    dateTimePickerEventDate.Value.Year,
                    dateTimePickerEventDate.Value.Month,
                    dateTimePickerEventDate.Value.Day,
                    (int)numericUpDownTimeHour.Value,
                    (int)numericUpDownTimeMinute.Value,
                    textBoxEventDecription.Text);

                if (DateTime.Compare(eventDateTime, DateTime.Now) < 0)
                {
                    recurringEvent.NotificationSent = true;
                }

                recurringEvents.Add(recurringEvent);
            }
            else
            {
                CalendarEvent newEvent = new CalendarEvent(
                    textBoxEventName.Text,
                    dateTimePickerEventDate.Value.Year,
                    dateTimePickerEventDate.Value.Month,
                    dateTimePickerEventDate.Value.Day,
                    (int)numericUpDownTimeHour.Value,
                    (int)numericUpDownTimeMinute.Value,
                    textBoxEventDecription.Text);

                if (DateTime.Compare(eventDateTime, DateTime.Now) < 0)
                {
                    newEvent.NotificationSent = true;
                }

                events.Add(newEvent);
            }

            eventTracker.Add(trackedEvent);
            totalEventsMade++;
            BuildCalendar(drawCalendar);
            buttonCreateEvent.Enabled = false;
            textBoxEventName.Text = "";
            textBoxEventDecription.Text = "";
            checkBoxRecurringEvent.Checked = false;
            UpdateList();
            UpdateStats();
        }

        private void DeleteEvent()
        {
            if (listBoxEvents.SelectedIndex >= 0)//something is selected
            {
                RemoveEventFromLists(listBoxEvents.SelectedIndex);
            }
        }

        private void RemoveEventFromLists(int eventNumber)
        {
            for (int i = events.Count - 1; i >= 0; i--)
            {
                if (DateTime.Compare(events[i].DateInfo, eventTracker[eventNumber].DateInfo) == 0 && events[i].Name == eventTracker[eventNumber].Name)
                {
                    events.RemoveAt(i);
                }
            }
            for (int j = recurringEvents.Count - 1; j >= 0; j--)
            {
                if (DateTime.Compare(recurringEvents[j].DateInfo, eventTracker[eventNumber].DateInfo) == 0 && recurringEvents[j].Name == eventTracker[eventNumber].Name)
                {
                    recurringEvents.RemoveAt(j);
                }
            }
            eventTracker.RemoveAt(eventNumber);
            UpdateList();
            BuildCalendar(drawCalendar);
            labelDescription.Text = "";
            buttonDelete.Enabled = false;
            buttonFinishEvent.Enabled = false;
            UpdateStats();
        }

        private void BuildExtraStudentFeatures()
        {
            timerFocus = new Timer();
            timerFocus.Interval = 1000;
            timerFocus.Tick += new EventHandler(timerFocus_Tick);

            BuildFocusTab();
            BuildStatsTab();
            BuildNotesTab();

            buttonFinishEvent = new Button();
            buttonFinishEvent.BackColor = Color.LightGreen;
            buttonFinishEvent.Enabled = false;
            buttonFinishEvent.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            buttonFinishEvent.Location = new Point(8, 374);
            buttonFinishEvent.Name = "buttonFinishEvent";
            buttonFinishEvent.Size = new Size(235, 25);
            buttonFinishEvent.Text = "Mark Selected Done";
            buttonFinishEvent.UseVisualStyleBackColor = false;
            buttonFinishEvent.Click += new EventHandler(buttonFinishEvent_Click);
            tabPage3.Controls.Add(buttonFinishEvent);

            UpdateStats();
            UpdateNoteEventChoices();
        }

        private void BuildFocusTab()
        {
            tabPageFocus = new TabPage();
            tabPageFocus.Name = "tabPageFocus";
            tabPageFocus.Text = "Focus";
            tabPageFocus.BackColor = Color.Black;

            Label labelTitle = new Label();
            labelTitle.Text = "Focus Mode";
            labelTitle.ForeColor = Color.White;
            labelTitle.Font = new Font("Microsoft Sans Serif", 22F, FontStyle.Bold);
            labelTitle.Location = new Point(40, 35);
            labelTitle.Size = new Size(350, 45);

            Label labelSubtitle = new Label();
            labelSubtitle.Text = "Use the timer or stopwatch to track your study sessions.";
            labelSubtitle.ForeColor = Color.Gray;
            labelSubtitle.Location = new Point(42, 80);
            labelSubtitle.Size = new Size(400, 20);

            labelFocusTime = new Label();
            labelFocusTime.Text = "00:25:00";
            labelFocusTime.ForeColor = Color.LightGreen;
            labelFocusTime.Font = new Font("Consolas", 36F, FontStyle.Bold);
            labelFocusTime.Location = new Point(40, 110);
            labelFocusTime.Size = new Size(350, 70);

            Label labelMode = new Label();
            labelMode.Text = "Mode";
            labelMode.ForeColor = Color.White;
            labelMode.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            labelMode.Location = new Point(45, 195);
            labelMode.Size = new Size(50, 20);

            radioTimer = new RadioButton();
            radioTimer.Text = "Timer";
            radioTimer.ForeColor = Color.White;
            radioTimer.Checked = true;
            radioTimer.Location = new Point(100, 195);

            radioStopWatch = new RadioButton();
            radioStopWatch.Text = "Stopwatch";
            radioStopWatch.ForeColor = Color.White;
            radioStopWatch.Location = new Point(185, 195);

            Label labelMinutes = new Label();
            labelMinutes.Text = "Duration (minutes)";
            labelMinutes.ForeColor = Color.White;
            labelMinutes.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            labelMinutes.Location = new Point(45, 230);
            labelMinutes.Size = new Size(130, 20);

            numericFocusMinutes = new NumericUpDown();
            numericFocusMinutes.Minimum = 1;
            numericFocusMinutes.Maximum = 240;
            numericFocusMinutes.Value = 25;
            numericFocusMinutes.Location = new Point(185, 228);
            numericFocusMinutes.Size = new Size(70, 20);
            numericFocusMinutes.ValueChanged += new EventHandler(numericFocusMinutes_ValueChanged);

            buttonFocusStart = new Button();
            buttonFocusStart.Text = "Start";
            buttonFocusStart.BackColor = Color.FromArgb(40, 120, 40);
            buttonFocusStart.ForeColor = Color.White;
            buttonFocusStart.FlatStyle = FlatStyle.Flat;
            buttonFocusStart.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            buttonFocusStart.Location = new Point(45, 275);
            buttonFocusStart.Size = new Size(120, 45);
            buttonFocusStart.Click += new EventHandler(buttonFocusStart_Click);

            buttonFocusStop = new Button();
            buttonFocusStop.Text = "Stop";
            buttonFocusStop.BackColor = Color.FromArgb(160, 40, 40);
            buttonFocusStop.ForeColor = Color.White;
            buttonFocusStop.FlatStyle = FlatStyle.Flat;
            buttonFocusStop.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            buttonFocusStop.Location = new Point(185, 275);
            buttonFocusStop.Size = new Size(120, 45);
            buttonFocusStop.Click += new EventHandler(buttonFocusStop_Click);

            tabPageFocus.Controls.Add(labelTitle);
            tabPageFocus.Controls.Add(labelSubtitle);
            tabPageFocus.Controls.Add(labelFocusTime);
            tabPageFocus.Controls.Add(labelMode);
            tabPageFocus.Controls.Add(labelMinutes);
            tabPageFocus.Controls.Add(numericFocusMinutes);
            tabPageFocus.Controls.Add(radioTimer);
            tabPageFocus.Controls.Add(radioStopWatch);
            tabPageFocus.Controls.Add(buttonFocusStart);
            tabPageFocus.Controls.Add(buttonFocusStop);
            tabControl.Controls.Add(tabPageFocus);
        }

        private void BuildStatsTab()
        {
            tabPageStats = new TabPage();
            tabPageStats.Name = "tabPageStats";
            tabPageStats.Text = "Stats";

            Label labelTitle = new Label();
            labelTitle.Text = "Your Progress";
            labelTitle.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold);
            labelTitle.Location = new Point(40, 10);
            labelTitle.Size = new Size(400, 30);

            Label labelExplain = new Label();
            labelExplain.Text = "Track how much time you spend studying and how many tasks you complete. Set goals below and see if you hit them.";
            labelExplain.Location = new Point(40, 42);
            labelExplain.Size = new Size(450, 35);
            labelExplain.ForeColor = Color.Gray;

            Label labelGoal1 = new Label();
            labelGoal1.Text = "Focus goal (minutes)";
            labelGoal1.Location = new Point(40, 90);
            labelGoal1.Size = new Size(140, 20);

            numericGoalMinutes = new NumericUpDown();
            numericGoalMinutes.Minimum = 1;
            numericGoalMinutes.Maximum = 10000;
            numericGoalMinutes.Value = 60;
            numericGoalMinutes.Location = new Point(190, 88);
            numericGoalMinutes.ValueChanged += new EventHandler(goal_ValueChanged);

            Label labelGoal2 = new Label();
            labelGoal2.Text = "Task done goal (%)";
            labelGoal2.Location = new Point(40, 120);
            labelGoal2.Size = new Size(140, 20);

            numericGoalPercent = new NumericUpDown();
            numericGoalPercent.Minimum = 1;
            numericGoalPercent.Maximum = 100;
            numericGoalPercent.Value = 75;
            numericGoalPercent.Location = new Point(190, 118);
            numericGoalPercent.ValueChanged += new EventHandler(goal_ValueChanged);

            labelStats = new Label();
            labelStats.BorderStyle = BorderStyle.Fixed3D;
            labelStats.Location = new Point(40, 155);
            labelStats.Size = new Size(430, 250);
            labelStats.Font = new Font("Microsoft Sans Serif", 11F);

            tabPageStats.Controls.Add(labelTitle);
            tabPageStats.Controls.Add(labelExplain);
            tabPageStats.Controls.Add(labelGoal1);
            tabPageStats.Controls.Add(numericGoalMinutes);
            tabPageStats.Controls.Add(labelGoal2);
            tabPageStats.Controls.Add(numericGoalPercent);
            tabPageStats.Controls.Add(labelStats);
            tabControl.Controls.Add(tabPageStats);
        }

        private void BuildNotesTab()
        {
            tabPageNotes = new TabPage();
            tabPageNotes.Name = "tabPageNotes";
            tabPageNotes.Text = "Notes";

            listBoxNotes = new ListBox();
            listBoxNotes.Location = new Point(25, 25);
            listBoxNotes.Size = new Size(250, 250);
            listBoxNotes.SelectedIndexChanged += new EventHandler(listBoxNotes_SelectedIndexChanged);

            Label labelTitle = new Label();
            labelTitle.Text = "Title";
            labelTitle.Location = new Point(300, 25);

            textBoxNoteTitle = new TextBox();
            textBoxNoteTitle.Location = new Point(300, 45);
            textBoxNoteTitle.Size = new Size(280, 22);

            Label labelAttach = new Label();
            labelAttach.Text = "Attach to event";
            labelAttach.Location = new Point(300, 80);
            labelAttach.Size = new Size(150, 20);

            comboBoxNoteEvents = new ComboBox();
            comboBoxNoteEvents.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxNoteEvents.Location = new Point(300, 100);
            comboBoxNoteEvents.Size = new Size(280, 22);

            richTextBoxNote = new RichTextBox();
            richTextBoxNote.Location = new Point(300, 145);
            richTextBoxNote.Size = new Size(400, 210);

            Button buttonSaveNote = new Button();
            buttonSaveNote.Text = "Save Note";
            buttonSaveNote.Location = new Point(300, 370);
            buttonSaveNote.Size = new Size(115, 35);
            buttonSaveNote.Click += new EventHandler(buttonSaveNote_Click);

            Button buttonNewNote = new Button();
            buttonNewNote.Text = "Clear";
            buttonNewNote.Location = new Point(430, 370);
            buttonNewNote.Size = new Size(115, 35);
            buttonNewNote.Click += new EventHandler(buttonNewNote_Click);

            Button buttonDeleteNote = new Button();
            buttonDeleteNote.Text = "Delete Note";
            buttonDeleteNote.Location = new Point(560, 370);
            buttonDeleteNote.Size = new Size(115, 35);
            buttonDeleteNote.Click += new EventHandler(buttonDeleteNote_Click);

            labelNoteBody = new Label();
            labelNoteBody.Text = "Select a note to view it, or type a title and note to save a new one.";
            labelNoteBody.Location = new Point(25, 295);
            labelNoteBody.Size = new Size(250, 120);

            tabPageNotes.Controls.Add(listBoxNotes);
            tabPageNotes.Controls.Add(labelTitle);
            tabPageNotes.Controls.Add(textBoxNoteTitle);
            tabPageNotes.Controls.Add(labelAttach);
            tabPageNotes.Controls.Add(comboBoxNoteEvents);
            tabPageNotes.Controls.Add(richTextBoxNote);
            tabPageNotes.Controls.Add(buttonSaveNote);
            tabPageNotes.Controls.Add(buttonNewNote);
            tabPageNotes.Controls.Add(buttonDeleteNote);
            tabPageNotes.Controls.Add(labelNoteBody);
            tabControl.Controls.Add(tabPageNotes);
        }

        private void UpdateFocusLabel()
        {
            if (usingStopWatch)
            {
                labelFocusTime.Text = FormatSeconds(focusSecondsPassed);
            }
            else
            {
                labelFocusTime.Text = FormatSeconds(focusSecondsLeft);
            }
        }

        private string FormatSeconds(int seconds)
        {
            int hours = seconds / 3600;
            int minutes = (seconds % 3600) / 60;
            int leftSeconds = seconds % 60;
            return hours.ToString("00") + ":" + minutes.ToString("00") + ":" + leftSeconds.ToString("00");
        }

        private void UpdateStats()
        {
            if (labelStats == null)
            {
                return;
            }

            int focusMinutes = totalFocusSeconds / 60;
            int percentDone = 0;
            if (totalEventsMade > 0)
            {
                percentDone = (totalEventsFinished * 100) / totalEventsMade;
            }

            string focusGoalText = "not reached";
            if (focusMinutes >= numericGoalMinutes.Value)
            {
                focusGoalText = "reached";
            }

            string percentGoalText = "not reached";
            if (percentDone >= numericGoalPercent.Value)
            {
                percentGoalText = "reached";
            }

            labelStats.Text = "Total focus time: " + FormatSeconds(totalFocusSeconds) + "\n\n" +
                              "Events created: " + totalEventsMade + "\n" +
                              "Events finished: " + totalEventsFinished + "\n" +
                              "Percent finished: " + percentDone + "%\n\n" +
                              "Focus goal: " + numericGoalMinutes.Value + " minutes - " + focusGoalText + "\n" +
                              "Task goal: " + numericGoalPercent.Value + "% - " + percentGoalText;
        }

        private void UpdateNoteEventChoices()
        {
            if (comboBoxNoteEvents == null)
            {
                return;
            }

            string oldText = "";
            if (comboBoxNoteEvents.SelectedItem != null)
            {
                oldText = comboBoxNoteEvents.SelectedItem.ToString();
            }

            comboBoxNoteEvents.Items.Clear();
            comboBoxNoteEvents.Items.Add("(no event)");
            for (int i = 0; i < eventTracker.Count; i++)
            {
                comboBoxNoteEvents.Items.Add(eventTracker[i].Name + " on " + eventTracker[i].DateInfo.ToShortDateString());
            }
            comboBoxNoteEvents.SelectedIndex = 0;

            for (int i = 0; i < comboBoxNoteEvents.Items.Count; i++)
            {
                if (comboBoxNoteEvents.Items[i].ToString() == oldText)
                {
                    comboBoxNoteEvents.SelectedIndex = i;
                }
            }
        }

        private void UpdateNoteList()
        {
            listBoxNotes.Items.Clear();
            for (int i = 0; i < notes.Count; i++)
            {
                listBoxNotes.Items.Add(notes[i].ToString());
            }
        }

        private void UpdateEditEventList()
        {
            if (listBoxEditEvents == null)
            {
                return;
            }

            listBoxEditEvents.Items.Clear();
            for (int i = 0; i < eventTracker.Count; i++)
            {
                listBoxEditEvents.Items.Add(eventTracker[i].Name + " on " + eventTracker[i].DateInfo.ToShortDateString());
            }
        }

        private void LoadEventIntoEditor(int index)
        {
            if (index < 0 || index >= eventTracker.Count)
            {
                return;
            }

            CalendarEvent selectedEvent = eventTracker[index];
            editingEventIndex = index;

            textBoxEditEventName.Text = selectedEvent.Name;
            textBoxEditEventDescription.Text = selectedEvent.Discription;
            dateTimePickerEditEventDate.Value = selectedEvent.DateInfo.Date;
            numericUpDownEditTimeHour.Value = selectedEvent.DateInfo.Hour;
            numericUpDownEditTimeMinute.Value = selectedEvent.DateInfo.Minute;

            bool isRecurring = false;
            for (int i = 0; i < recurringEvents.Count; i++)
            {
                if (DateTime.Compare(recurringEvents[i].DateInfo, selectedEvent.DateInfo) == 0 &&
                    recurringEvents[i].Name == selectedEvent.Name)
                {
                    isRecurring = true;
                    break;
                }
            }

            checkBoxEditRecurringEvent.Checked = isRecurring;
            buttonSaveEventChanges.Enabled = !string.IsNullOrWhiteSpace(textBoxEditEventName.Text);
        }

        private void SaveEditedEvent()
        {
            if (editingEventIndex < 0 || editingEventIndex >= eventTracker.Count)
            {
                MessageBox.Show("Please select an event to edit.", "No Event Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime editedDateTime = new DateTime(
                dateTimePickerEditEventDate.Value.Year,
                dateTimePickerEditEventDate.Value.Month,
                dateTimePickerEditEventDate.Value.Day,
                (int)numericUpDownEditTimeHour.Value,
                (int)numericUpDownEditTimeMinute.Value,
                0);

            if (DateTime.Compare(editedDateTime, DateTime.Now) < 0 && !checkBoxMakePastEvents.Checked)
            {
                MessageBox.Show("The edited event cannot be in the past unless that setting is enabled.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            for (int i = 0; i < eventTracker.Count; i++)
            {
                if (i == editingEventIndex)
                {
                    continue;
                }

                if (eventTracker[i].Name == textBoxEditEventName.Text &&
                    DateTime.Compare(eventTracker[i].DateInfo, editedDateTime) == 0)
                {
                    MessageBox.Show("Another event already has that same name and date.", "Duplicate Event", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            CalendarEvent oldTrackedEvent = eventTracker[editingEventIndex];

            bool oldWasRecurring = false;
            int oldRecurringIndex = -1;
            int oldEventIndex = -1;

            for (int i = 0; i < recurringEvents.Count; i++)
            {
                if (DateTime.Compare(recurringEvents[i].DateInfo, oldTrackedEvent.DateInfo) == 0 &&
                    recurringEvents[i].Name == oldTrackedEvent.Name)
                {
                    oldWasRecurring = true;
                    oldRecurringIndex = i;
                    break;
                }
            }

            if (!oldWasRecurring)
            {
                for (int i = 0; i < events.Count; i++)
                {
                    if (DateTime.Compare(events[i].DateInfo, oldTrackedEvent.DateInfo) == 0 &&
                        events[i].Name == oldTrackedEvent.Name)
                    {
                        oldEventIndex = i;
                        break;
                    }
                }
            }

            bool newIsRecurring = checkBoxEditRecurringEvent.Checked;

            eventTracker[editingEventIndex].Name = textBoxEditEventName.Text;
            eventTracker[editingEventIndex].Discription = textBoxEditEventDescription.Text;
            eventTracker[editingEventIndex].DateInfo = editedDateTime;

            if (oldWasRecurring && newIsRecurring)
            {
                recurringEvents[oldRecurringIndex].Name = textBoxEditEventName.Text;
                recurringEvents[oldRecurringIndex].Discription = textBoxEditEventDescription.Text;
                recurringEvents[oldRecurringIndex].DateInfo = editedDateTime;
            }
            else if (!oldWasRecurring && !newIsRecurring)
            {
                events[oldEventIndex].Name = textBoxEditEventName.Text;
                events[oldEventIndex].Discription = textBoxEditEventDescription.Text;
                events[oldEventIndex].DateInfo = editedDateTime;
            }
            else if (oldWasRecurring && !newIsRecurring)
            {
                recurringEvents.RemoveAt(oldRecurringIndex);
                events.Add(new CalendarEvent(
                    textBoxEditEventName.Text,
                    editedDateTime.Year,
                    editedDateTime.Month,
                    editedDateTime.Day,
                    editedDateTime.Hour,
                    editedDateTime.Minute,
                    textBoxEditEventDescription.Text));
            }
            else if (!oldWasRecurring && newIsRecurring)
            {
                events.RemoveAt(oldEventIndex);
                recurringEvents.Add(new RecurringEvent(
                    textBoxEditEventName.Text,
                    editedDateTime.Year,
                    editedDateTime.Month,
                    editedDateTime.Day,
                    editedDateTime.Hour,
                    editedDateTime.Minute,
                    textBoxEditEventDescription.Text));
            }

            UpdateList();
            UpdateEditEventList();
            UpdateNoteEventChoices();
            BuildCalendar(drawCalendar);

            listBoxEditEvents.SelectedIndex = editingEventIndex;
            MessageBox.Show("Event updated successfully.", "Edit Event", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void listBoxEditEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxEditEvents.SelectedIndex >= 0)
            {
                LoadEventIntoEditor(listBoxEditEvents.SelectedIndex);
            }
        }

        private void buttonSaveEventChanges_Click(object sender, EventArgs e)
        {
            SaveEditedEvent();
        }

        private void textBoxEditEventName_TextChanged(object sender, EventArgs e)
        {
            buttonSaveEventChanges.Enabled = !string.IsNullOrWhiteSpace(textBoxEditEventName.Text) &&
                                             editingEventIndex >= 0;
        }

        //ALL EVENT TRIGGERS BELOW
        private void buttonFocusStart_Click(object sender, EventArgs e)
        {
            usingStopWatch = radioStopWatch.Checked;
            focusRunning = true;
            focusSecondsPassed = 0;
            if (!usingStopWatch)
            {
                focusSecondsLeft = (int)numericFocusMinutes.Value * 60;
            }
            UpdateFocusLabel();
            timerFocus.Start();
        }

        private void buttonFocusStop_Click(object sender, EventArgs e)
        {
            timerFocus.Stop();
            focusRunning = false;
            if (usingStopWatch)
            {
                totalFocusSeconds += focusSecondsPassed;
            }
            else
            {
                totalFocusSeconds += ((int)numericFocusMinutes.Value * 60) - focusSecondsLeft;
            }
            focusSecondsPassed = 0;
            focusSecondsLeft = (int)numericFocusMinutes.Value * 60;
            UpdateFocusLabel();
            UpdateStats();
        }



        private void timerFocus_Tick(object sender, EventArgs e)
        {
            if (!focusRunning)
            {
                return;
            }

            if (usingStopWatch)
            {
                focusSecondsPassed++;
            }
            else
            {
                focusSecondsLeft--;
                if (focusSecondsLeft <= 0)
                {
                    totalFocusSeconds += (int)numericFocusMinutes.Value * 60;
                    timerFocus.Stop();
                    focusRunning = false;
                    focusSecondsLeft = 0;
                    MessageBox.Show("Timer finished!", "Focus Mode", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateStats();
                }
            }
            UpdateFocusLabel();
        }

        private void numericFocusMinutes_ValueChanged(object sender, EventArgs e)
        {
            if (!focusRunning)
            {
                focusSecondsLeft = (int)numericFocusMinutes.Value * 60;
                UpdateFocusLabel();
            }
        }

        private void goal_ValueChanged(object sender, EventArgs e)
        {
            UpdateStats();
        }

        private void buttonFinishEvent_Click(object sender, EventArgs e)
        {
            if (listBoxEvents.SelectedIndex >= 0)
            {
                totalEventsFinished++;
                RemoveEventFromLists(listBoxEvents.SelectedIndex);
                UpdateStats();
            }
        }

        private void buttonSaveNote_Click(object sender, EventArgs e)
        {
            if (textBoxNoteTitle.Text == "")
            {
                MessageBox.Show("Please type a title for the note.", "Missing Title", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string attachedEvent = "";
            if (comboBoxNoteEvents.SelectedItem != null && comboBoxNoteEvents.SelectedItem.ToString() != "(no event)")
            {
                attachedEvent = comboBoxNoteEvents.SelectedItem.ToString();
            }

            bool isEditing = listBoxNotes.SelectedIndex >= 0 &&
                             notes[listBoxNotes.SelectedIndex].Title == textBoxNoteTitle.Text;

            if (isEditing)
            {
                notes[listBoxNotes.SelectedIndex].Words = richTextBoxNote.Text;
                notes[listBoxNotes.SelectedIndex].AttachedEvent = attachedEvent;
            }
            else
            {
                for (int i = 0; i < notes.Count; i++)
                {
                    if (notes[i].Title == textBoxNoteTitle.Text)
                    {
                        MessageBox.Show("A note with the title \"" + textBoxNoteTitle.Text + "\" already exists. Please use a different title.", "Duplicate Title", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                notes.Add(new StudentNote(textBoxNoteTitle.Text, richTextBoxNote.Text, attachedEvent));
            }

            UpdateNoteList();
        }

        private void buttonNewNote_Click(object sender, EventArgs e)
        {
            listBoxNotes.SelectedIndex = -1;
            textBoxNoteTitle.Text = "";
            richTextBoxNote.Text = "";
            comboBoxNoteEvents.SelectedIndex = 0;
            labelNoteBody.Text = "New note";
        }

        private void buttonDeleteNote_Click(object sender, EventArgs e)
        {
            if (listBoxNotes.SelectedIndex >= 0)
            {
                notes.RemoveAt(listBoxNotes.SelectedIndex);
                UpdateNoteList();
                buttonNewNote_Click(sender, e);
            }
        }

        private void listBoxNotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxNotes.SelectedIndex >= 0)
            {
                StudentNote note = notes[listBoxNotes.SelectedIndex];
                textBoxNoteTitle.Text = note.Title;
                richTextBoxNote.Text = note.Words;
                labelNoteBody.Text = note.Words;
                comboBoxNoteEvents.SelectedIndex = 0;
                for (int i = 0; i < comboBoxNoteEvents.Items.Count; i++)
                {
                    if (comboBoxNoteEvents.Items[i].ToString() == note.AttachedEvent)
                    {
                        comboBoxNoteEvents.SelectedIndex = i;
                    }
                }
            }
        }

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
            AddEvent();
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
            timerCalendar.Stop();
            Notification();
            BuildCalendar(drawCalendar);
            timerCalendar.Start();
        }

        private void listBoxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxEvents.SelectedIndex >= 0)//something is selected
            {
                labelDescription.Text = eventTracker[listBoxEvents.SelectedIndex].Discription;
                buttonDelete.Enabled = true;
                buttonFinishEvent.Enabled = true;
            }
            else if (listBoxEvents.SelectedIndex < 0)
            {
                labelDescription.Text = "";
                buttonDelete.Enabled = false;
                buttonFinishEvent.Enabled = false;
            }
        }

        private void tabControlInCalendar_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateList();

            if (tabControlInCalendar.SelectedTab == tabPage4)
            {
                UpdateEditEventList();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DeleteEvent();
        }
    }
}
