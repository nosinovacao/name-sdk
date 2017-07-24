using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NAME.Core;
using NAME.SelfHost.Shared;

namespace NAME.SelfHost.Kestrel
{
    internal class KestrelServer : ISelfHostServer
    {
        private IWebHost webHost;
        private NAMEKestrelConfiguration nameConfiguration;
        private IFilePathMapper pathMapper;

        public int? Port { get; private set; }

        public KestrelServer(NAMEKestrelConfiguration nameConfiguration, IFilePathMapper pathMapper)
        {
            this.pathMapper = pathMapper;
            this.nameConfiguration = nameConfiguration;
        }

        public bool Start(int port, NAMESettings settings)
        {
            try
            {
                this.webHost = new WebHostBuilder()
                    .UseKestrel()
                    .UseStartup<ServerStartup>()
                    .UseUrls($"http://{this.nameConfiguration.AddressToListenOn}:{port}/{this.nameConfiguration.ManifestUriPrefix.TrimStart('/').TrimEnd('/')}/")
                    .ConfigureServices((services) =>
                    {
                        services.AddSingleton(settings);
                        services.AddSingleton(this.nameConfiguration);
                        services.AddSingleton(this.pathMapper);
                    })
                    .Build();

                this.webHost.Start();

                this.Port = port;

                return true;
            }
            catch (Microsoft.AspNetCore.Server.Kestrel.Internal.Networking.UvException)
            {
                this.webHost?.Dispose();
                return false;
            }
        }


        public void Dispose()
        {
            this.webHost?.Dispose();
        }

    }
}
