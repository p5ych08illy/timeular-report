// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using timeular_api;
using timeular_report;

class Program
{

    static async Task<int> Main(string[] args)
    {
        if (args.Length < 1 || string.IsNullOrEmpty(args[0]) || args[0] == "--help")
        {
            PrintUsage();
            return -1;
        }

        var round = args.Skip(1).Contains("--round");
        var monthArgs = args.Skip(1).Where(arg => arg != "--round").ToArray();
        var date = DateTime.Now.AddMonths(-1);
        if (monthArgs.Length > 1 ||
            (monthArgs.Length == 1 && !DateTime.TryParse(monthArgs[0], out date)))
        {
            PrintUsage();
            return -1;
        }

        IConfiguration config = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .AddUserSecrets<Program>()
              .Build();

        // Get values from the config, given their key and their target type.
        var settings = config.GetRequiredSection("Settings").Get<Settings>();
        if (settings != null)
        {
            using var api = TimeularAPIFactory.Create();
            var success = await api.SignInAsync(new timeular_api.SignIn.SignInRequest { apiKey = settings.ApiKey, apiSecret = settings.ApiSecret });
            if (success)
            {
                var report = new Report(api);
                await report.GenerateReport(args[0], date, round);
                api.Logout();
            }
        }

        return 0;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("usage: timeular-report.exe folder [month] [--round]" + Environment.NewLine +
                          "  --round  Round times to the nearest 15 minutes (disabled by default)." + Environment.NewLine +
                          "Example: timeular-report.exe C:\\ 2023.08 --round"
              );
    }
}


