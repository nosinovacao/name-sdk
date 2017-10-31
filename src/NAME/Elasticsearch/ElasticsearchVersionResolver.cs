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

                var getResponseTask = request.GetResponseAsync();
                var readWriteTimeout = this.ConnectTimeout + this.ReadWriteTimeout;

                readWriteTimeout = readWriteTimeout < 1000 ? 1000 : readWriteTimeout;

                //If Elasticsearch takes to long to return a response, when readWriteTimeout passes, the request will be aborted
                var completedTask = await Task.WhenAny(getResponseTask, Task.Delay(readWriteTimeout)).ConfigureAwait(false);
                if (completedTask == getResponseTask)
                {
                    using (response = await getResponseTask.ConfigureAwait(false) as HttpWebResponse)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            var body = await reader.ReadToEndAsync();
                            var version = string.Empty;

                            version = this.DeserializeJsonResponse(body);
                            versions.Add(DependencyVersion.Parse(version));
                        }
                    }
                }
                else
                {
                    request.Abort();
                    throw new NAMEException($"{SupportedDependencies.Elasticsearch}: Timed out, the server accepted the connection but did not send a response.");
                }
            }
            catch (WebException e)
            {
                throw new DependencyNotReachableException($"{SupportedDependencies.Elasticsearch}: {e.Message}");
            }

            return versions;
        }

        private string DeserializeJsonResponse(string result)
        {
            try
            {
                var jsonResult = Json.Json.Parse(result);
                return jsonResult["version"]["number"];
            }
            catch (Exception e)
            {
                throw new VersionParsingException(result, e.Message);
            }
        }
    }
}