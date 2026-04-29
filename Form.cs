// Added helper methods and a hardened SaveEditedEvent implementation.
// Insert these methods inside the Form class (near other private helpers).

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
    for (int i = 0; i < recurringEvents.Count; i++)
    {
        if (recurringEvents[i].Name == trackedEvent.Name &&
            DateTime.Compare(recurringEvents[i].DateInfo, trackedEvent.DateInfo) == 0)
        {
            return i;
        }
    }
    return -1;
}

private void SaveEditedEvent()
{
    // add these Debug.WriteLine calls at start of SaveEditedEvent (temporary)
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
    if (!oldWasRecurring)
    {
        oldEventIndex = FindEventIndexByTracked(oldTrackedEvent);
    }

    bool newIsRecurring = false;
    var editCb2 = this.Controls.Find("comboBoxEditRecurrence", true).FirstOrDefault() as ComboBox;
    if (editCb2 != null && editCb2.SelectedItem != null)
    {
        newIsRecurring = editCb2.SelectedItem.ToString().ToLower() != "once";
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

    // Perform backing collection updates, guarded
    if (oldWasRecurring && newIsRecurring)
    {
        // update existing recurring backing item
        recurringEvents[oldRecurringIndex].Name = textBoxEditEventName.Text;
        recurringEvents[oldRecurringIndex].Discription = textBoxEditEventDescription.Text;
        recurringEvents[oldRecurringIndex].DateInfo = editedDateTime;
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
            textBoxEditEventDescription.Text));
    }

    UpdateList();
    UpdateEditEventList();
    UpdateNoteEventChoices();
    BuildCalendar(drawCalendar);

    listBoxEditEvents.SelectedIndex = editingEventIndex;
    MessageBox.Show("Event updated successfully.", "Edit Event", MessageBoxButtons.OK, MessageBoxIcon.Information);
}