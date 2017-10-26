using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NAME.Elasticsearch
{
    internal class ElasticsearchVersionResolver : ConnectedVersionResolver
    {
        private IConnectionStringProvider connectionStringProvider;

        public ElasticsearchVersionResolver(IConnectionStringProvider connectionStringProvider, int connectTimeout, int readWriteTimeout)
            : base(connectTimeout, readWriteTimeout)
        {
            this.connectionStringProvider = connectionStringProvider;
        }

        public override async Task<IEnumerable<DependencyVersion>> GetVersions()
        {
            var versions = new List<DependencyVersion>();
            var connectionString = string.Empty;

            if (!this.connectionStringProvider.TryGetConnectionString(out connectionString))
            {
                throw new ConnectionStringNotFoundException(this.connectionStringProvider.ToString());
            }

            using (var client = this.OpenHttpClient())
            {
                try
                {
                    var result = await client.GetStringAsync(connectionString).ConfigureAwait(false);
                    versions.Add(DependencyVersion.Parse(this.DeserializeJsonResponse(result)));
                }
                catch (Exception e)
                {
                    throw new NAMEException(e.Message); 
                }
            }

            return versions;
        }

        private string DeserializeJsonResponse(string result)
        {
            
            var jsonResult = Json.Json.Parse(result);
            return jsonResult["version"]["number"];
        }
    }
}