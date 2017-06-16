using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocketHttpListener.Net;
using System.Text;
using NAME.Hosting.Shared;
using NAME.SelfHost.Shared;
using static NAME.Hosting.Shared.DependenciesUtils;
using static NAME.Utils.LogUtils;

namespace NAME.SelfHost.HttpListener
{
    /// <summary>
    /// Provides a mechanism to start a self hosted HTTP server serving the NAME endpoints using an HttpListener server.
    /// </summary>
    public static class NAMEServer
    {
        /// <summary>
        /// Enables the NAME self host server.
        /// </summary>
        /// <param name="nameConfigBuilder">The name configuration builder.</param>
        /// <returns>
        /// Returns an <see cref="IDisposable" /> proxy object containing the Parsed Dependencies and a way to stop the server.
        /// </returns>
        /// <exception cref="NAMEException">Error parsing the dependencies file.</exception>
        /// <exception cref="DependenciesCheckException">Happens when <see cref="NAMEBaseConfiguration.ThrowOnDependenciesFail" /> is set to true and dependencies check fail.</exception>
        public static SelfHostResult EnableName(Action<NAMEHttpListenerConfiguration> nameConfigBuilder)
        {
            var nameConfiguration = new NAMEHttpListenerConfiguration();
            nameConfigBuilder?.Invoke(nameConfiguration);

            var pathMapper = new StaticFilePathMapper();
            
            var server = new HttpListenerServer(nameConfiguration, pathMapper);
            var parsedDependencies = SelfHostInitializer.Initialize(server, pathMapper, nameConfiguration);

            return new SelfHostResult(parsedDependencies, server);
        }
        
    }
}
