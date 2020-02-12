using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NAME.AspNetCore;

namespace NAME.DummyService
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
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
            services.AddMvc(options => options.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
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
