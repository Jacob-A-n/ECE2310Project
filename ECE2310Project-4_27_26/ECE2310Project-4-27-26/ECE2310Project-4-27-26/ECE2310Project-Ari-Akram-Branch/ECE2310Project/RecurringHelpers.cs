using System;
using System.Collections.Generic;

namespace ECE2310Project
{
    public enum RecurrenceType
    {
        Once,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    public static class RecurrenceHelper
    {
        // Generate occurrence datetimes for a recurring event between viewStart (inclusive) and viewEnd (inclusive)
        // Keeps time of the original start value.
        public static IEnumerable<DateTime> GenerateOccurrences(DateTime start, RecurrenceType type, DateTime viewStart, DateTime viewEnd, int interval = 1, DateTime? until = null)
        {
            if (type == RecurrenceType.Once)
            {
                if (start >= viewStart && start <= viewEnd) yield return start;
                yield break;
            }

            DateTime stop = until ?? viewEnd;

            if (start > stop) yield break;

            DateTime candidate = start;

            // Fast-forward candidate to be >= viewStart
            if (candidate < viewStart)
            {
                switch (type)
                {
                    case RecurrenceType.Daily:
                        {
                            var daysDiff = (viewStart.Date - candidate.Date).Days;
                            var steps = Math.Max(0, daysDiff / interval);
                            candidate = candidate.AddDays(steps * interval);
                            while (candidate < viewStart) candidate = candidate.AddDays(interval);
                            break;
                        }
                    case RecurrenceType.Weekly:
                        {
                            var daysDiff = (viewStart.Date - candidate.Date).Days;
                            var weeks = Math.Max(0, (daysDiff / 7) / interval);
                            candidate = candidate.AddDays(weeks * interval * 7);
                            while (candidate < viewStart) candidate = candidate.AddDays(interval * 7);
                            break;
                        }
                    case RecurrenceType.Monthly:
                        {
                            while (candidate < viewStart)
                            {
                                candidate = candidate.AddMonths(interval);
                                if (candidate > stop) yield break;
                            }
                            break;
                        }
                    case RecurrenceType.Yearly:
                        {
                            while (candidate < viewStart)
                            {
                                candidate = candidate.AddYears(interval);
                                if (candidate > stop) yield break;
                            }
                            break;
                        }
                }
            }

            while (candidate <= viewEnd && candidate <= stop)
            {
                yield return candidate;
                switch (type)
                {
                    case RecurrenceType.Daily:
                        candidate = candidate.AddDays(interval);
                        break;
                    case RecurrenceType.Weekly:
                        candidate = candidate.AddDays(7 * interval);
                        break;
                    case RecurrenceType.Monthly:
                        candidate = candidate.AddMonths(interval);
                        break;
                    case RecurrenceType.Yearly:
                        candidate = candidate.AddYears(interval);
                        break;
                }
            }
        }
    }
}