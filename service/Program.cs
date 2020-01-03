using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//using authService.Contexts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace authService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Console.WriteLine($"environment name: '{envName}'");

            var envHostFilename = $"hosting.{envName}.json";
            Console.WriteLine($"environment host filename: '{envHostFilename}'");

            var configurationBuilder = new ConfigurationBuilder()
              .AddJsonFile("hosting.json", optional: false, reloadOnChange: true);

            if (envName != null)
            {
                configurationBuilder
                    .AddJsonFile($"hosting.{envName}.json", optional: true, reloadOnChange: true);
            }

            var configuration = configurationBuilder
              .AddCommandLine(args)
              .Build();

            //var appConfig = new Settings.Application();
            //configuration.GetSection("App").Bind(appConfig);

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.SetBasePath(Directory.GetCurrentDirectory());
                        config.AddJsonFile($"secrets/appSettings.{envName}.json", optional: true, reloadOnChange: true);
                        config.AddCommandLine(args);
                    })
                .UseConfiguration(configuration)
                .UseStartup<Startup>()
                .Build();
        }
    }
}
