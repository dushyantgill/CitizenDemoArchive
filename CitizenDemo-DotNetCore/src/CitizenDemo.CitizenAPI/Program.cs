using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;

namespace CitizenDemo.CitizenAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            ILogger logger = LoggerFactory.Create(builder => builder.AddConsole().AddDebug()).CreateLogger<Program>();
            try
            {
                logger.LogDebug("Citizen API Startup: starting");
                host.Run();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Citizen API Startup: starting failed");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole(configure =>
                    {
                        configure.FormatterName = ConsoleFormatterNames.Systemd;
                    });
                })
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole(configure =>
                    {
                        configure.FormatterName = ConsoleFormatterNames.Systemd;
                    }));
                    var logger = loggerFactory.CreateLogger(typeof(Program));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
