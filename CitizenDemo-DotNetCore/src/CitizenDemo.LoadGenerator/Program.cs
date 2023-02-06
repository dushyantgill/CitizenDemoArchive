using CitizenDemo.LoadGenerator.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitizenDemo.LoadGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            ILogger logger = LoggerFactory.Create(builder => builder.AddConsole().AddDebug()).CreateLogger<Program>();
            try
            {
                logger.LogDebug("LoadGenerator Startup: starting");
                host.Run();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "LoadGenerator Startup: starting failed");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole(configure =>
                    {
                        configure.FormatterName = Microsoft.Extensions.Logging.Console.ConsoleFormatterNames.Systemd;
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
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole(configure =>
                    {
                        configure.FormatterName = ConsoleFormatterNames.Systemd;
                    }));
                    var logger = loggerFactory.CreateLogger(typeof(Program));

                    services.AddHttpClient();

                    //CitizenSync calls CitizenAPI to create, delete, and search citizens
                    CitizenServiceSettings citizenServiceSettings = configuration.GetSection("CitizenServiceSettings").Get<CitizenServiceSettings>();
                    services.AddSingleton(citizenServiceSettings);

                    var serviceName = "CitizenDemo.LoadGenerator";
                    services.AddOpenTelemetry()
                        .WithTracing(builder => builder
                            .SetResourceBuilder(ResourceBuilder
                                .CreateDefault()
                                .AddService(serviceName: serviceName))
                            .AddOtlpExporter(options => options
                                .Endpoint = new Uri(configuration.GetSection("MonitoringSettings")["TraceExportEndpoint"]))
                            .AddHttpClientInstrumentation())
                        .StartWithHost();

                    services.AddHostedService<Worker>();
                });
    }
}
