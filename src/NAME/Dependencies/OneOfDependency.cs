using NAME.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAME.Json;
using NAME.Core;

namespace NAME.Dependencies
{
    internal class OneOfDependency : Dependency
    {
        public OneOfDependency(IList<Dependency> dependencies)
        {
            this.Dependencies = dependencies;
        }
        public IList<Dependency> Dependencies { get; private set; }


        public override async Task<DependencyCheckStatus> GetStatus()
        {
            if (!this.Dependencies.Any())
                return new DependencyCheckStatus(NAMEStatusLevel.Warn, message: "A OneOf condition dependency must have child dependencies.");
            try
            {
                var allDependenciesTasks = this.Dependencies.Select(d => d.GetStatus());

                var results = await Task.WhenAll(allDependenciesTasks).ConfigureAwait(false);
                var bestStatus = results.Min(s => s.CheckStatus);

                string message = "Everything is ok!";
                Exception innerException = null;
                if (bestStatus != NAMEStatusLevel.Ok)
                {
                    message = string.Join(Environment.NewLine, results.Select(s => s.Message));
                    var exceptions = results.Where(s => s.InnerException != null).Select(s => s.InnerException);
                    if (exceptions == null || exceptions.Count() == 0)
                        innerException = new NAMEException("None of the dependencies matched the version", NAMEStatusLevel.Error);
                    else
                        innerException = new AggregateException(exceptions);
                }

                return new DependencyCheckStatus(bestStatus, message: message, innerException: innerException);

            }
            catch (NAMEException ex)
            {
                return new DependencyCheckStatus(ex.StatusLevel, message: ex.Message, innerException: ex);
            }
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder("OneOf:");
            foreach (var dependency in this.Dependencies)
            {
                result.Append($" '{dependency.ToString()}'");
            }
            return result.ToString();
        }

        internal async override Task<JsonNode> ToJson()
        {
            var rootJson = new JsonClass();
            var arrayJson = new JsonArray();
            rootJson.Add("oneOf", arrayJson);
            foreach (var inner in this.Dependencies)
            {
                arrayJson.Add(await inner.ToJson().ConfigureAwait(false));
            }

            return rootJson;
        }
    }
}
