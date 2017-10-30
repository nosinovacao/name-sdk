using NAME.Core;
using NAME.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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

            HttpWebResponse response;
            try
            {
                var request = this.GetHttpWebRequest(connectionString, SupportedDependencies.Elasticsearch.ToString());
                response = await request.GetResponseAsync() as HttpWebResponse;
            }
            catch (Exception e)
            {
                throw new DependencyNotReachableException($"{SupportedDependencies.Elasticsearch}: {e.Message}");
            }
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var body = await reader.ReadToEndAsync();
                var version = string.Empty;
                try
                {
                    version = this.DeserializeJsonResponse(body);
                    versions.Add(DependencyVersion.Parse(version));
                }
                catch (Exception e)
                {
                    throw new VersionParsingException(version, e.Message);
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