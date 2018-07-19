using Microsoft.Extensions.Configuration;
using NAME.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.AspNetCore
{
    internal class AspNetCoreConfigurationConnectionStringProvider : IConnectionStringProvider
    {
        private readonly IConfiguration configuration;
        private readonly string connectionStringKey;

        public AspNetCoreConfigurationConnectionStringProvider(IConfiguration configuration, string connectionStringKey)
        {
            if (string.IsNullOrWhiteSpace(connectionStringKey))
            {
                throw new ArgumentException("Value must be set.", nameof(connectionStringKey));
            }

            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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
