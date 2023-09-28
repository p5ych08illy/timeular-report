// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using timeular_report;

IConfiguration config = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json")
      .AddUserSecrets<Program>()
      .Build();

// Get values from the config, given their key and their target type.
Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

Console.WriteLine($"key:{settings.ApiKey}");