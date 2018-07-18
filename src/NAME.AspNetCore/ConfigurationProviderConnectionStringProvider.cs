using Microsoft.Extensions.Configuration;
using NAME.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.AspNetCore
{
    internal class ConfigurationProviderConnectionStringProvider : IConnectionStringProvider
    {
        private readonly IConfigurationProvider configurationProvider;
        private readonly string connectionStringKey;

        public ConfigurationProviderConnectionStringProvider(IConfigurationProvider configurationProvider, string connectionStringKey)
        {
            if (string.IsNullOrWhiteSpace(connectionStringKey))
            {
                throw new ArgumentException("message", nameof(connectionStringKey));
            }

            this.configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            this.connectionStringKey = connectionStringKey;
        }

        public bool TryGetConnectionString(out string connectionString)
        {
            return this.configurationProvider.TryGet(this.connectionStringKey, out connectionString);
        }
    }
}
