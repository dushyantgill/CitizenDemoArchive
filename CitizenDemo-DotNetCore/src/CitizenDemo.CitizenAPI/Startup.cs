using CitizenDemo.CitizenAPI.Data;
using CitizenDemo.CitizenAPI.Services;
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
using Prometheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CitizenDemo.CitizenAPI
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
                CitizenCollectionName = Configuration.GetSection("DatabaseSettings")["CitizenCollectionName"]
            });
            services.AddSingleton<ICitizenRepository, CitizenRepository>();

            //CitizenAPI calls ResourceAPI using HttpClient + Polly for retry w/ exponential backoff
            services.AddHttpClient<IResourceService, ResourceService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));
            services.Configure<ResourceServiceSettings>(Configuration.GetSection(nameof(ResourceServiceSettings)));
            services.AddSingleton<IResourceServiceSettings>(sp => sp.GetRequiredService<IOptions<ResourceServiceSettings>>().Value);
            services.AddSingleton<IResourceService, ResourceService>();

            services.AddControllers();

            //CitizenAPI enables CORS
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
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
