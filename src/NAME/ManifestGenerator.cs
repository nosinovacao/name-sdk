using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NAME.Json;
using System.IO;
using NAME.Core;

namespace NAME
{
    /// <summary>
    /// Provides mechanisms to generate a manifest.
    /// </summary>
    public static class ManifestGenerator
    {
        /// <summary>
        /// Generates a json representation of the manifest;
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        /// <param name="dependencies">The dependencies.</param>
        /// <returns>Returns a task representing the asynchronous operation. The result contains the generated json manifest.</returns>
        public static async Task<string> GenerateJson(string name, string version, ParsedDependencies dependencies)
        {
            JsonClass rootJson = new JsonClass();
            rootJson.Add("nameVersion", Constants.NAME_ASSEMBLY_VERSION);
            rootJson.Add("name", name);

            if (version.Count(c => c == '.') > 2)
                version = string.Join(".", version.Split('.').Take(3));

            rootJson.Add("version", version);

            List<Task<JsonNode>> infrastructureTasks = new List<Task<JsonNode>>();
            foreach (var dependency in dependencies.InfrastructureDependencies)
            {
                infrastructureTasks.Add(dependency.ToJson());
            }
            List<Task<JsonNode>> serviceTasks = new List<Task<JsonNode>>();
            foreach (var dependency in dependencies.ServiceDependencies)
            {
                serviceTasks.Add(dependency.ToJson());
            }
            await Task.WhenAll(Task.WhenAll(serviceTasks), Task.WhenAll(infrastructureTasks)).ConfigureAwait(false);

            JsonArray infrastructureDependencies = new JsonArray();
            JsonArray serviceDependencies = new JsonArray();
            foreach (var task in serviceTasks)
            {
                serviceDependencies.Add(task.Result);
            }
            foreach (var task in infrastructureTasks)
            {
                infrastructureDependencies.Add(task.Result);
            }
            rootJson.Add("infrastructure_dependencies", infrastructureDependencies);
            rootJson.Add("service_dependencies", serviceDependencies);

            return rootJson.ToString();
        }
    }
}
