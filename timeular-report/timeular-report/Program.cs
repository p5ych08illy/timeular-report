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
                var date = DateTime.Now.AddMonths(-1);

                if (args.Length > 1 && DateTime.TryParse(args[1], out var input))
                {
                    date = input.Date;
                }

                var report = new Report(api);
                await report.GenerateReport(args[0], date);
                api.Logout();
            }
        }

        return 0;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("usage: timeular-report.exe folder [month]" + Environment.NewLine +
                          "Example: timeular-report.exe C:\\ 2023.08"
              );
    }
}


