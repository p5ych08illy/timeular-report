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

        internal async Task GenerateReport(string path, DateTime month)
        {
            var firstDayOfMonth = new DateTime(month.Year, month.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1);

            var data = await api.GetAllData(firstDayOfMonth, lastDayOfMonth);

            var groupByDay = from entry in data.timeEntries
                             orderby entry.duration.startedAt
                             group entry by entry.duration.startedAt.Day;

            var entries = new List<(string day, DateTime start, DateTime end)>();

            foreach (var day in groupByDay)
            {
                var last = day.First();
                var prev = day.First();

                var date = last.duration.startedAt.Date.ToShortDateString();

                for (var i = 1; i < day.Count(); i++)
                {
                    var entry = day.ElementAt(i);

                    if (entry.activity.name == "Lunch")
                    {
                        if (entry.duration.startedAt - prev.duration.startedAt >= TimeSpan.FromMinutes(15))
                            entries.Add((date, RoundToNearest(prev.duration.startedAt), RoundToNearest(entry.duration.startedAt)));
                        prev = day.ElementAtOrDefault(i + 1);
                    }
                    else if (last.activity.name != "Lunch" && (entry.duration.startedAt - last.duration.stoppedAt) > TimeSpan.FromMinutes(15))
                    {
                        entries.Add((date, RoundToNearest(prev.duration.startedAt), RoundToNearest(last.duration.stoppedAt)));
                        prev = entry;
                    }

                    last = entry;
                }

                if (prev != null && prev.activity.name != "Lunch" && day.Last().activity.name != "Lunch")
                    entries.Add((date, RoundToNearest(prev.duration.startedAt), RoundToNearest(day.Last().duration.stoppedAt)));

            }

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
            return RoundToNearest(time.ToLocalTime(), TimeSpan.FromMinutes(15)).ToShortTimeString();
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
