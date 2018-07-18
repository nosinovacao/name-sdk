using Microsoft.Extensions.Configuration;
using NAME.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.AspNetCore
{
    internal class ConfigurationProviderConnectionStringProvider : IConnectionStringProvider
    {
        private readonly IConfiguration configuration;
        private readonly string connectionStringKey;

        public ConfigurationProviderConnectionStringProvider(IConfiguration configurationProvider, string connectionStringKey)
        {
            if (string.IsNullOrWhiteSpace(connectionStringKey))
            {
                throw new ArgumentException("message", nameof(connectionStringKey));
            }

            this.configuration = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            this.connectionStringKey = connectionStringKey;
        }

        public bool TryGetConnectionString(out string connectionString)
        {
            try
            {
                connectionString = this.configuration[this.connectionStringKey];
                return connectionString != null;
            }
            catch
            {
                connectionString = null;
                return false;
            }
        }
    }
}
