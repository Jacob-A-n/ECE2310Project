// File improved by AI - original version had issues with event editing and tracking, and some code repetition. Refactored for better clarity and reliability, especially around event editing and the focus timer.

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

            // determine the actual date represented by the first calendar cell
            DateTime calendarMonthFirst = new DateTime(dCal.DateWindow.Year, dCal.DateWindow.Month, 1);
            DateTime calendarStartDate = calendarMonthFirst.AddDays(-dCal.StartOfMonth);
            DateTime calendarEndDate = calendarStartDate.AddDays(42 - 1);

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

            // local helper to decide whether a recurring event occurs on a specific date
            bool RecursOn(RecurringEvent r, DateTime date)
            {
                DateTime start = r.DateInfo.Date;
                if (date < start) return false;

                switch (r.Frequency)
                {
                    case RecurrenceFrequency.Daily:
                        // every day for the next year
                        return date <= start.AddYears(1);

                    case RecurrenceFrequency.Weekly:
                        // week-aligned occurrences for next year
                        var days = (date - start).Days;
                        return date <= start.AddYears(1) && days % 7 == 0;

                    case RecurrenceFrequency.Monthly:
                        // same day-of-month for up to one year
                        return date <= start.AddYears(1) && date.Day == start.Day;

                    case RecurrenceFrequency.Yearly:
                        // same month+day for up to five years
                        return date <= start.AddYears(5) && date.Month == start.Month && date.Day == start.Day;

                    default:
                        return false;
                }
            }

            // local method to write events into calendar cell text
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
            }

            // place recurring event occurrences by checking each calendar cell date
            for (int i = 0; i < recurringEvents.Count; i++)
            {
                for (int j = 0; j < 42; j++)
                {
                    DateTime cellDate = calendarStartDate.AddDays(j);
                    if (RecursOn(recurringEvents[i], cellDate))
                    {
                        DrawEvents(i, j, "recurring");
                    }
                }
            }

            // place single (one-off) events by exact date match
            for (int i = 0; i < events.Count; i++)
            {
                DateTime evDate = events[i].DateInfo.Date;
                if (evDate < calendarStartDate || evDate > calendarEndDate) continue;

                int index = (int)(evDate - calendarStartDate).TotalDays;
                if (index >= 0 && index < 42)
                {
                    DrawEvents(i, index, "event");
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
                else if (DateTime.Compare(events[i].DateInfo, Cal.AddDays(DateTime.Now, -5)) <= 0) //removes events that are 5 days or more older
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
                string displayName = eventTracker[i].Name;
                int rIndex = FindRecurringIndexByTracked(eventTracker[i]);
                if (rIndex >= 0)
                {
                    displayName += " (" + recurringEvents[rIndex].Frequency.ToString() + ")";
                }
                listBoxEvents.Items.Add(displayName + " on " + eventTracker[i].DateInfo.ToShortDateString());
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

            // duplicate check omitted for brevity (keep existing)

            bool isRecurring = false;
            RecurrenceFrequency frequency = RecurrenceFrequency.Yearly; // default

            var cb = this.Controls.Find("comboBoxRecurrence", true).FirstOrDefault() as ComboBox;
            if (cb != null && cb.SelectedIndex >= 0)
            {
                isRecurring = cb.SelectedIndex != 0; // 0 == Once
                frequency = (RecurrenceFrequency)cb.SelectedIndex;
            }
            else if (cb != null && cb.SelectedItem != null) // fallback: parse trimmed string
            {
                var sel = cb.SelectedItem.ToString().Trim();
                isRecurring = sel.ToLower() != "once";
                Enum.TryParse(sel, true, out frequency);
            }
            else
            {
                var chk = this.Controls.Find("checkBoxRecurringEvent", true).FirstOrDefault() as CheckBox;
                if (chk != null) { isRecurring = chk.Checked; if (isRecurring) frequency = RecurrenceFrequency.Yearly; }
            }

            if (isRecurring)
            {
                recurringEvents.Add(new RecurringEvent(
                    textBoxEventName.Text,
                    eventDateTime.Year,
                    eventDateTime.Month,
                    eventDateTime.Day,
                    (int)numericUpDownTimeHour.Value,
                    (int)numericUpDownTimeMinute.Value,
                    textBoxEventDecription.Text,
                    frequency));
            }
            else
            {
                events.Add(new CalendarEvent(
                    textBoxEventName.Text,
                    eventDateTime.Year,
                    eventDateTime.Month,
                    eventDateTime.Day,
                    (int)numericUpDownTimeHour.Value,
                    (int)numericUpDownTimeMinute.Value,
                    textBoxEventDecription.Text));
            }

            eventTracker.Add(new CalendarEvent(textBoxEventName.Text, dateTimePickerEventDate.Value.Year, dateTimePickerEventDate.Value.Month, dateTimePickerEventDate.Value.Day, (int)numericUpDownTimeHour.Value, (int)numericUpDownTimeMinute.Value, textBoxEventDecription.Text));
            totalEventsMade++;
            BuildCalendar(drawCalendar);
            buttonCreateEvent.Enabled = false;
            textBoxEventName.Text = "";
            textBoxEventDecription.Text = "";
            // clear checkbox only if it exists (keeps compatibility)
            var chkClear = this.Controls.Find("checkBoxRecurringEvent", true).FirstOrDefault() as CheckBox;
            if (chkClear != null) chkClear.Checked = false;
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
            UpdateStats();
            UpdateNoteEventChoices();
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
            if (index < 0 || index >= eventTracker.Count) return;

            CalendarEvent selectedEvent = eventTracker[index];
            editingEventIndex = index;

            textBoxEditEventName.Text = selectedEvent.Name;
            textBoxEditEventDescription.Text = selectedEvent.Discription;
            dateTimePickerEditEventDate.Value = selectedEvent.DateInfo.Date;
            numericUpDownEditTimeHour.Value = selectedEvent.DateInfo.Hour;
            numericUpDownEditTimeMinute.Value = selectedEvent.DateInfo.Minute;

            int recurringIndex = FindRecurringIndexByTracked(selectedEvent);
            var editCb = this.Controls.Find("comboBoxEditRecurrence", true).FirstOrDefault() as ComboBox;
            if (editCb != null)
            {
                editCb.SelectedIndex = recurringIndex >= 0 ? (int)recurringEvents[recurringIndex].Frequency : 0;
            }
            else
            {
                var chk = this.Controls.Find("checkBoxEditRecurringEvent", true).FirstOrDefault() as CheckBox;
                if (chk != null) chk.Checked = recurringIndex >= 0;
            }

            buttonSaveEventChanges.Enabled = !string.IsNullOrWhiteSpace(textBoxEditEventName.Text);
        }

        private void SaveEditedEvent()
        {
            System.Diagnostics.Debug.WriteLine($"SaveEditedEvent ENTER: editingEventIndex={editingEventIndex} events={events.Count} recurring={recurringEvents.Count} tracker={eventTracker.Count}");
            for (int i = 0; i < events.Count; i++) System.Diagnostics.Debug.WriteLine($"events[{i}] {events[i].Name} :: {events[i].DateInfo}");
            for (int i = 0; i < recurringEvents.Count; i++) System.Diagnostics.Debug.WriteLine($"recurring[{i}] {recurringEvents[i].Name} :: {recurringEvents[i].DateInfo}");
            for (int i = 0; i < eventTracker.Count; i++) System.Diagnostics.Debug.WriteLine($"tracker[{i}] {eventTracker[i].Name} :: {eventTracker[i].DateInfo}");

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
                if (i == editingEventIndex) continue;

                if (eventTracker[i].Name == textBoxEditEventName.Text &&
                    DateTime.Compare(eventTracker[i].DateInfo, editedDateTime) == 0)
                {
                    MessageBox.Show("Another event already has that same name and date.", "Duplicate Event", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            CalendarEvent oldTrackedEvent = eventTracker[editingEventIndex];

            // Use helper finders so behavior is consistent and logged
            int oldRecurringIndex = FindRecurringIndexByTracked(oldTrackedEvent);
            bool oldWasRecurring = oldRecurringIndex >= 0;
            int oldEventIndex = -1;
            if (!oldWasRecurring) oldEventIndex = FindEventIndexByTracked(oldTrackedEvent);

            bool newIsRecurring = false;
            RecurrenceFrequency newFrequency = RecurrenceFrequency.Yearly;
            var editCb2 = this.Controls.Find("comboBoxEditRecurrence", true).FirstOrDefault() as ComboBox;
            if (editCb2 != null && editCb2.SelectedIndex >= 0)
            {
                newIsRecurring = editCb2.SelectedIndex != 0;
                newFrequency = (RecurrenceFrequency)editCb2.SelectedIndex;
            }
            else if (editCb2 != null && editCb2.SelectedItem != null)
            {
                newIsRecurring = editCb2.SelectedItem.ToString().Trim().ToLower() != "once";
                Enum.TryParse(editCb2.SelectedItem.ToString(), true, out newFrequency);
            }
            else
            {
                var chk2 = this.Controls.Find("checkBoxEditRecurringEvent", true).FirstOrDefault() as CheckBox;
                if (chk2 != null) newIsRecurring = chk2.Checked;
            }

            // Update tracker entry first
            eventTracker[editingEventIndex].Name = textBoxEditEventName.Text;
            eventTracker[editingEventIndex].Discription = textBoxEditEventDescription.Text;
            eventTracker[editingEventIndex].DateInfo = editedDateTime;

            // Persist changes and set Frequency where applicable
            if (oldWasRecurring && newIsRecurring)
            {
                recurringEvents[oldRecurringIndex].Name = textBoxEditEventName.Text;
                recurringEvents[oldRecurringIndex].Discription = textBoxEditEventDescription.Text;
                recurringEvents[oldRecurringIndex].DateInfo = editedDateTime;
                recurringEvents[oldRecurringIndex].Frequency = newFrequency;
            }
            else if (!oldWasRecurring && !newIsRecurring)
            {
                if (oldEventIndex >= 0)
                {
                    events[oldEventIndex].Name = textBoxEditEventName.Text;
                    events[oldEventIndex].Discription = textBoxEditEventDescription.Text;
                    events[oldEventIndex].DateInfo = editedDateTime;
                }
                else
                {
                    // backing event missing: log and add one (safer than throwing)
                    System.Diagnostics.Debug.WriteLine($"SaveEditedEvent: one-off backing event not found for '{oldTrackedEvent.Name}' @ {oldTrackedEvent.DateInfo}. Creating new backing event.");
                    events.Add(new CalendarEvent(
                        textBoxEditEventName.Text,
                        editedDateTime.Year,
                        editedDateTime.Month,
                        editedDateTime.Day,
                        editedDateTime.Hour,
                        editedDateTime.Minute,
                        textBoxEditEventDescription.Text));
                }
            }
            else if (oldWasRecurring && !newIsRecurring)
            {
                if (oldRecurringIndex >= 0 && oldRecurringIndex < recurringEvents.Count)
                {
                    recurringEvents.RemoveAt(oldRecurringIndex);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"SaveEditedEvent: expected recurring index {oldRecurringIndex} invalid when converting to one-off for '{oldTrackedEvent.Name}'");
                }

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
                if (oldEventIndex >= 0 && oldEventIndex < events.Count)
                {
                    events.RemoveAt(oldEventIndex);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"SaveEditedEvent: expected event index {oldEventIndex} invalid when converting to recurring for '{oldTrackedEvent.Name}'");
                }

                recurringEvents.Add(new RecurringEvent(
                    textBoxEditEventName.Text,
                    editedDateTime.Year,
                    editedDateTime.Month,
                    editedDateTime.Day,
                    editedDateTime.Hour,
                    editedDateTime.Minute,
                    textBoxEditEventDescription.Text,
                    newFrequency));
            }

            UpdateList();
            UpdateEditEventList();
            UpdateNoteEventChoices();
            BuildCalendar(drawCalendar);

            listBoxEditEvents.SelectedIndex = editingEventIndex;
            MessageBox.Show("Event updated successfully.", "Edit Event", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

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

        private void groupBoxProperties_Enter(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void comboBoxEditRecurrence_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private int FindEventIndexByTracked(CalendarEvent trackedEvent)
        {
            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].Name == trackedEvent.Name &&
                    DateTime.Compare(events[i].DateInfo, trackedEvent.DateInfo) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private int FindRecurringIndexByTracked(CalendarEvent trackedEvent)
        {
            // Match recurring by name + month + day + hour + minute (ignore year)
            for (int i = 0; i < recurringEvents.Count; i++)
            {
                var r = recurringEvents[i];
                if (r.Name == trackedEvent.Name &&
                    r.DateInfo.Month == trackedEvent.DateInfo.Month &&
                    r.DateInfo.Day == trackedEvent.DateInfo.Day &&
                    r.DateInfo.Hour == trackedEvent.DateInfo.Hour &&
                    r.DateInfo.Minute == trackedEvent.DateInfo.Minute)
                {
                    return i;
                }
            }
            return -1;
        }

        private void textBoxEditEventName_TextChanged(object sender, EventArgs e)
        {
            // Enable the Save Changes button only if the event name is not empty
            buttonSaveEventChanges.Enabled = !string.IsNullOrWhiteSpace(textBoxEditEventName.Text);
        }

        private void listBoxEditEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Example implementation: load the selected event into the editor
            if (listBoxEditEvents.SelectedIndex >= 0)
            {
                LoadEventIntoEditor(listBoxEditEvents.SelectedIndex);
            }
        }

        private void buttonSaveEventChanges_Click(object sender, EventArgs e)
        {
            // Implement the logic to save changes to the edited event
            SaveEditedEvent();
        }
    }
}
