# timeular-report
Generates csv report by day for a given month (or last month if second parameter is missing) with EARLY's (formerly Timeular) API.
A Timeular_Report_yyyy_MM.csv file will be generated in the given folder. 

# Usage
`timeular-report --help`
```
Usage: timeular-report.exe folder [month] [--round]

Example: timeular-report.exe C:\\ 2023.08
Example with rounding: timeular-report.exe C:\\ 2023.08 --round
```

Rounding is disabled by default. Pass `--round` to round the start and end times
to the nearest 15 minutes. Totals and averages use the same times as the report.
Without a month, the report covers the previous month.

Activities marked as not counting towards work hours in EARLY are treated as
breaks, regardless of their name. This uses `excludedFromWorkHours` from the web
client's `internal/v8/time-tracking/activities` endpoint, because the public v3/v4
activity endpoints omit this setting. This internal endpoint may change without
notice. If a reported activity's classification is missing, report generation
fails instead of silently counting breaks as work.
