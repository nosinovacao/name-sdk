using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using NAME.AspNetCore;
using NAME.Core;
using NAME.Hosting.Shared;
using NAME.SelfHost.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static NAME.Hosting.Shared.DependenciesUtils;

namespace NAME.SelfHost.Kestrel
{
    internal class ServerStartup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IFilePathMapper pathMapper, NAMEKestrelConfiguration configuration, NAMESettings settings)
        {
            app.UseMiddleware<NAMEMiddleware>(configuration, settings, pathMapper);
        }
    }
}
