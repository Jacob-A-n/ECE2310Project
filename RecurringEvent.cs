// Fix constructor logic to advance yearly notifications when the initial DateInfo is in the past.
// Replace the constructor body in RecurringEvent class with this:

public RecurringEvent(string name, int year, int month, int day, int hour = 0, int minute = 0, string discription = "") : base(name, year, month, day, hour, minute, discription)
{
    // If the provided date is in the past relative to now, advance the notification forward.
    // Previous condition used > 0 (future) and the comment indicated the opposite.
    if (DateTime.Compare(DateInfo, DateTime.Now) < 0)
    {
        // Use the parameterless UpdateNotificationTime which increments the year correctly.
        UpdateNotificationTime();
    }
}