using NAME.Core;
using NAME.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.ConnectionStrings
{
    /// <summary>
    /// Represents a Connection string provider that extracts it from a JSON file
    /// </summary>
    /// <seealso cref="NAME.Core.IConnectionStringProvider" />
    public class JsonPathConnectionStringProvider : IConnectionStringProvider
    {
        private static IJsonPathValueSystem valueSystem = new SimpleJsonPathValueSystem();

        private Lazy<JsonPathContext> context = new Lazy<JsonPathContext>(() => new JsonPathContext() { ValueSystem = valueSystem });

        private string jsonFilePath;
        private string jsonPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPathConnectionStringProvider" /> class.
        /// </summary>
        /// <param name="jsonFilePath">The json file path.</param>
        /// <param name="jsonPath">The json path expression of the connection string.</param>
        public JsonPathConnectionStringProvider(string jsonFilePath, string jsonPath)
        {
            this.jsonFilePath = jsonFilePath;
            this.jsonPath = jsonPath;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="connectionString">A <see cref="string" /> containing the connection string.</param>
        /// <returns>Returns true if the connection string was fetched successfully.</returns>
        public bool TryGetConnectionString(out string connectionString)
        {
            connectionString = null;
            try
            {
                if (!File.Exists(this.jsonFilePath))
                    return false;


                string fileContents = File.ReadAllText(this.jsonFilePath);
                JsonNode rootNode = Json.Json.Parse(fileContents);
                JsonNode fetchedNode = this.context.Value.Select(rootNode, this.jsonPath)?.First() as JsonNode;

                connectionString = fetchedNode?.Value;
                return true;
            }
            catch (Exception)
            {
                Trace.TraceInformation($"Could not fetch the connection string from the file {this.jsonFilePath} with the json path {this.jsonPath}.");
                return false;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Locator: {SupportedConnectionStringLocators.JSONPath}, XPath: {this.jsonPath}, File: {this.jsonFilePath}";
        }
    }
}
