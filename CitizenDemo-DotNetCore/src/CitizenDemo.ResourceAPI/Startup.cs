using CitizenDemo.ResourceAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CitizenDemo.ResourceAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole(configure =>
            {
                configure.FormatterName = ConsoleFormatterNames.Systemd;
            }));
            var logger = loggerFactory.CreateLogger(typeof(Program));

            services.AddSingleton<IDatabaseSettings>(new DatabaseSettings
            {
                ConnectionString = Configuration.GetSection("DatabaseSettings")["ConnectionString"],
                DatabaseName = Configuration.GetSection("DatabaseSettings")["DatabaseName"],
                ResourceCollectionName = Configuration.GetSection("DatabaseSettings")["ResourceCollectionName"]
            });
            services.AddSingleton<IResourceRepository, ResourceRepository>();

            services.AddControllers();

            //CitizenAPI enables CORS
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            var serviceName = "CitizenDemo.ResourceAPI";
            services.AddOpenTelemetry()
                .WithTracing(builder => builder
                    .SetResourceBuilder(ResourceBuilder
                        .CreateDefault()
                        .AddService(serviceName: serviceName))
                    .AddOtlpExporter(options => options
                        .Endpoint = new Uri(Configuration.GetSection("MonitoringSettings")["TraceExportEndpoint"]))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation())
                .StartWithHost();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseMetricServer();

            app.UseHttpMetrics();

            app.UseCors("MyPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
