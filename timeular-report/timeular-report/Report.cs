using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using timeular_api;
using timeular_api.Reports;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

            var entries = new List<(string day, string start, string end)>();

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
                        entries.Add((date, Format(prev.duration.startedAt), Format(entry.duration.startedAt)));
                        prev = day.ElementAtOrDefault(i + 1);
                    }
                    else if ((entry.duration.startedAt - last.duration.stoppedAt) > TimeSpan.FromMinutes(15))
                    {
                        entries.Add((date, Format(prev.duration.startedAt), Format(last.duration.stoppedAt)));
                        prev = entry;
                    }

                    last = entry;
                }

                if (prev != null && prev.activity.name != "Lunch")
                    entries.Add((date, Format(prev.duration.startedAt), Format(day.Last().duration.stoppedAt)));

            }

            var time = groupByDay.SelectMany(d => d.Where(e => e.activity.name != "Lunch").Select(e => (e.duration.stoppedAt - e.duration.startedAt).TotalMinutes)).Sum();
            var avg = time / (double)groupByDay.Count();

            var csv = new StringBuilder();
            csv.AppendLine("Date\tFrom\tTo");
            foreach (var item in entries)
            {
                csv.AppendLine($"{item.day}\t{item.start}\t{item.end}");
            }
            csv.AppendLine();
            csv.AppendLine($"Days: {groupByDay.Count()} ; Total Hours: {time / 60} ; AVG: {avg / 60}");
            File.WriteAllText(Path.Combine(path, $"Timeular_Report_{month.ToString("yyyy_MM")}.csv"), csv.ToString()); //TODO error handling
        }

        private string Format(DateTime time)
        {
            return RoundToNearest(time.ToLocalTime(), TimeSpan.FromMinutes(15)).ToShortTimeString();
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
