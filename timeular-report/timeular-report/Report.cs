using System.Text;
using timeular_api;

namespace timeular_report
{
    internal class Report
    {
        private ITimeularAPI api;

        public Report(ITimeularAPI api)
        {
            this.api = api;
        }

        internal async Task GenerateReport(string path, DateTime month, bool round = false)
        {
            var firstDayOfMonth = new DateTime(month.Year, month.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1);

            var data = await api.GetAllData(firstDayOfMonth, lastDayOfMonth);
            var activities = await api.GetAllActivities();
            var workHoursExclusions = activities.activities.Select(a => (a.id, a.excludedFromWorkHours))
                .Concat(activities.inactiveActivities.Select(a => (a.id, a.excludedFromWorkHours)))
                .Concat(activities.archivedActivities.Select(a => (a.id, a.excludedFromWorkHours)))
                .ToDictionary(a => a.id, a => a.excludedFromWorkHours);

            foreach (var entry in data.timeEntries)
            {
                if (!workHoursExclusions.TryGetValue(entry.activity.id, out var excluded) || !excluded.HasValue)
                    throw new InvalidOperationException($"Work-hours classification is missing for activity {entry.activity.id}.");
            }

            var groupByDay = from entry in data.timeEntries
                             orderby entry.duration.startedAt
                             group entry by entry.duration.startedAt.Day;

            var entries = new List<(string day, DateTime start, DateTime end)>();

            foreach (var day in groupByDay)
            {
                DateTime? start = null;
                DateTime? end = null;
                var date = day.First().duration.startedAt.Date.ToShortDateString();

                foreach (var entry in day)
                {
                    if (workHoursExclusions[entry.activity.id] == true)
                    {
                        if (start.HasValue && entry.duration.startedAt - start.Value >= TimeSpan.FromMinutes(15))
                            entries.Add((date, start.Value, entry.duration.startedAt));
                        start = null;
                        end = null;
                    }
                    else
                    {
                        if (start.HasValue && end.HasValue && entry.duration.startedAt - end.Value > TimeSpan.FromMinutes(15))
                        {
                            entries.Add((date, start.Value, end.Value));
                            start = null;
                        }
                        start ??= entry.duration.startedAt;
                        end = entry.duration.stoppedAt;
                    }
                }

                if (start.HasValue && end.HasValue)
                    entries.Add((date, start.Value, end.Value));

            }

            entries = entries.Select(e => (e.day,
                start: round ? RoundToNearest(e.start.ToLocalTime()) : e.start.ToLocalTime(),
                end: round ? RoundToNearest(e.end.ToLocalTime()) : e.end.ToLocalTime())).ToList();

            var time = entries.Select(e => (e.end - e.start).TotalMinutes)
                              .Sum();
            var avg = time / (double)groupByDay.Count();

            var csv = new StringBuilder();
            csv.AppendLine("Date\tFrom\tTo");
            foreach (var item in entries)
            {
                csv.AppendLine($"{item.day}\t{Format(item.start)}\t{Format(item.end)}");
            }
            csv.AppendLine();
            csv.AppendLine($"Days: {groupByDay.Count()} ; Total Hours: {time / 60} ; AVG: {avg / 60}");
            File.WriteAllText(Path.Combine(path, $"Timeular_Report_{month.ToString("yyyy_MM")}.csv"), csv.ToString()); //TODO error handling
        }

        private string Format(DateTime time)
        {
            return time.ToShortTimeString();
        }

        private DateTime RoundToNearest(DateTime dt)
        {
            return RoundToNearest(dt, TimeSpan.FromMinutes(15));
        }

        private DateTime RoundToNearest(DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            bool roundUp = delta > d.Ticks / 2;
            var offset = roundUp ? d.Ticks : 0;

            return new DateTime(dt.Ticks + offset - delta, dt.Kind);
        }
    }
}
