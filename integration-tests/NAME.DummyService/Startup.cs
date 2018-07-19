using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NAME.AspNetCore;

namespace NAME.DummyService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.HasValue)
                {
                    if (context.Request.Path.Value == "/endpoint/before/name/middleware/manifest")
                    {
                        context.Response.StatusCode = 200;
                        return;
                    }
                }
                await next();
            });

            app.UseNAME(config =>
            {
                config.APIName = "Dummy";
                config.APIVersion = "1.0.0";
                config.DependenciesFilePath = "dependencies.json";
                config.Configuration = Configuration;
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.HasValue)
                {
                    if (context.Request.Path.Value == "/not/the/real/manifest")
                    {
                        context.Response.StatusCode = 200;
                        return;
                    }
                }
                await next();
            });

            app.UseMvc();
        }
    }
}
