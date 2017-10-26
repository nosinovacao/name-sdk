using NAME.Core;
using NAME.Core.Exceptions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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

            using (var client = this.OpenHttpClient(SupportedDependencies.Elasticsearch.ToString()))
            {
                var result = await client.GetStringAsync(connectionString).ConfigureAwait(false);


                versions.Add(DependencyVersion.Parse(this.DeserializeJsonResponse(result)));
            }

            return versions;
        }

        private string DeserializeJsonResponse(string result)
        {
            var jsonResult = JsonConvert.DeserializeObject<ElasticResponseJson>(result);
            return jsonResult.version.number;
        }
    }
}