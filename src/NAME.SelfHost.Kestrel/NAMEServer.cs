using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Dependencies;
using NAME.Hosting.Shared;
using NAME.SelfHost.Shared;
using static NAME.Utils.LogUtils;

namespace NAME.SelfHost.Kestrel
{
    /// <summary>
    /// Provides a mechanism to start a self hosted HTTP server serving the NAME endpoints using the Kestrel server.
    /// </summary>
    public static class NAMEServer
    {
        /// <summary>
        /// Enables the NAME self host server.
        /// </summary>
        /// <param name="nameConfigBuilder">The name configuration builder.</param>
        /// <returns>Returns an <see cref="IDisposable"/> proxy object containing the Parsed Dependencies and a way to stop the server.</returns>
        /// <exception cref="NAMEException">Error parsing the dependencies file.</exception>
        /// <exception cref="DependenciesCheckException">Happens when <see cref="NAMEBaseConfiguration.ThrowOnDependenciesFail"/> is set to true and dependencies check fail.</exception>
        public static SelfHostResult EnableName(Action<NAMEKestrelConfiguration> nameConfigBuilder)
        {
            var config = new NAMEKestrelConfiguration();
            nameConfigBuilder?.Invoke(config);

            var pathMapper = new StaticFilePathMapper();

            var server = new KestrelServer(config, pathMapper);
            var parsedDependencies = SelfHostInitializer.Initialize(server, pathMapper, config);

            return new SelfHostResult(parsedDependencies, server);
        }


    }
}
