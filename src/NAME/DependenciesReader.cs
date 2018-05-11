using NAME.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NAME.Json;
using NAME.Core;
using NAME.ConnectionStrings;
using NAME.Dependencies;
using NAME.Registration;
using NAME.Core.Utils;

namespace NAME
{
    /// <summary>
    /// Provides mechanisms to read a NAME configuration file.
    /// </summary>
    public static class DependenciesReader
    {
        /// <summary>
        /// The maximum dependency depth
        /// </summary>
        public const int MAX_DEPENDENCY_DEPTH = 10;

        private static IDictionary<string, IVersionTranslator> dependencyVersionTranslators;

        static DependenciesReader()
        {
            dependencyVersionTranslators = new Dictionary<string, IVersionTranslator>(StringComparer.OrdinalIgnoreCase)
            {
                [SupportedDependencies.SqlServer.ToString()] = new SqlServer.SqlServerVersionTranslator(),
                ["windows"] = new OperatingSystem.WindowsVersionTranslator()
            };
        }

        /// <summary>
        /// Reads the configuration.
        /// </summary>
        /// <param name="dependenciesFile">The configuration file.</param>
        /// <param name="pathMapper">The path mapper.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// Returns the dependencies read from the file.
        /// </returns>
        /// <exception cref="NAMEException">An unhandled exception happened.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The configuration file was not found.</exception>
        public static ParsedDependencies ReadDependencies(string dependenciesFile, IFilePathMapper pathMapper, NAMESettings settings, NAMEContext context)
        {
            Guard.NotNull(settings, nameof(settings));
            if (context == null)
                context = new NAMEContext();

            string jsonContents = ReadJsonContents(pathMapper.MapPath(dependenciesFile));

            return ParseDependenciesFromString(jsonContents, pathMapper, settings, context);
        }

        private static string ReadJsonContents(string dependenciesFile)
        {
            if (!File.Exists(dependenciesFile))
            {
                string exceptionMessage = "The dependencies file was not found.";
                throw new NAMEException(exceptionMessage, new FileNotFoundException(exceptionMessage, dependenciesFile), NAMEStatusLevel.Warn);
            }

            string[] jsonLines = File.ReadAllLines(dependenciesFile);
            string jsonContents = string.Join(Environment.NewLine, jsonLines.Where(l => !l.TrimStart().StartsWith("//")));
            return jsonContents;
        }

        /// <summary>
        /// Reads the dependencies.
        /// </summary>
        /// <param name="configurationStream">The configuration stream.</param>
        /// <param name="pathMapper">The path mapper.</param>
        /// <param name="settings">The context information.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// Returns the dependencies read from the stream.
        /// </returns>
        /// <exception cref="NAMEException">Configuration file stream is not in readable state</exception>
        public static ParsedDependencies ReadDependencies(
            Stream configurationStream,
            IFilePathMapper pathMapper,
            NAMESettings settings,
            NAMEContext context)
        {
            Guard.NotNull(settings, nameof(settings));
            if (context == null)
                context = new NAMEContext();

            if (configurationStream.CanRead == false)
            {
                throw new NAMEException("Configuration file stream is not in readable state", NAMEStatusLevel.Warn);
            }

            using (StreamReader reader = new StreamReader(configurationStream))
            {
                var jsonContents = reader.ReadToEnd();
                return ParseDependenciesFromString(jsonContents, pathMapper, settings, context);
            }
        }

        private static ParsedDependencies ParseDependenciesFromString(
            string jsonContents,
            IFilePathMapper pathMapper,
            NAMESettings settings,
            NAMEContext context)
        {
            JsonNode rootNode = Json.Json.Parse(jsonContents);

            return new ParsedDependencies(
                HandleDependencyArray(rootNode?["infrastructure_dependencies"]?.AsArray, pathMapper, settings, context),
                HandleDependencyArray(rootNode?["service_dependencies"]?.AsArray, pathMapper, settings, context));
        }

        private static IList<Dependency> HandleDependencyArray(JsonArray dependencies, IFilePathMapper pathMapper, NAMESettings configuration, NAMEContext context, int depth = 0)
        {
            IList<Dependency> handledDependencies = new List<Dependency>();
            if (dependencies == null)
                return handledDependencies;

            foreach (JsonNode dependency in dependencies)
            {
                if (dependency.AsObject != null)
                    handledDependencies.Add(HandleDependency(dependency.AsObject, pathMapper, configuration, context, depth));
            }
            return handledDependencies;
        }

        private static Dependency HandleDependency(JsonClass dependency, IFilePathMapper pathMapper, NAMESettings configuration, NAMEContext context, int depth = 0)
        {
            if (depth == MAX_DEPENDENCY_DEPTH)
                throw new NAMEException($"Reached the maximum dependency recursion of {MAX_DEPENDENCY_DEPTH}.", NAMEStatusLevel.Warn);

            if (dependency == null)
                return null;
            var conditionObject = dependency["oneOf"];
            if (conditionObject != null)
            {
                depth++;
                return new OneOfDependency(HandleDependencyArray(conditionObject.AsArray, pathMapper, configuration, context, depth));
            }

            var minVersion = dependency["min_version"].Value;
            var maxVersion = dependency["max_version"].Value;
            var name = dependency["name"]?.Value;
            var type = dependency["type"]?.Value;
            var osName = dependency["os_name"]?.Value;
            type = string.IsNullOrEmpty(type) ? SupportedDependencies.Service.ToString() : type;
            if (!Enum.TryParse(type, out SupportedDependencies typedType))
                throw new NAMEException($"The dependency type {type} is not supported.", NAMEStatusLevel.Warn);

            VersionedDependency result;
            if (typedType == SupportedDependencies.OperatingSystem)
            {
                result = new OperatingSystemDependency()
                {
                    OperatingSystemName = osName,
                    MinimumVersion = ParseMinimumVersion(minVersion, osName),
                    MaximumVersion = ParseMaximumVersion(maxVersion, osName)
                };
            }
            else
            {
                var connectionStringProvider = ParseConnectionStringProvider(dependency["connection_string"], pathMapper);
                result = new ConnectedDependency(GetConnectedDependencyVersionResolver(typedType, connectionStringProvider, configuration, context))
                {
                    ConnectionStringProvider = connectionStringProvider,
                    MinimumVersion = ParseMinimumVersion(minVersion, type),
                    MaximumVersion = ParseMaximumVersion(maxVersion, type),
                    ShowConnectionStringInJson = configuration.ConnectedDependencyShowConnectionString
                };
            }

            result.Name = name;
            result.Type = typedType;
            return result;
        }

        private static DependencyVersion ParseMinimumVersion(string version, string dependencyType)
        {
            if (DependencyVersionParser.TryParse(version, false, out DependencyVersion parsedVersion))
                return parsedVersion;

            if (!dependencyVersionTranslators.ContainsKey(dependencyType))
                throw new VersionParsingException($"Could not parse the version of the dependency type {dependencyType}.");

            return dependencyVersionTranslators[dependencyType].Translate(version);
        }


        private static DependencyVersion ParseMaximumVersion(string version, string dependencyType)
        {
            if (DependencyVersionParser.TryParse(version, true, out DependencyVersion parsedVersion))
                return parsedVersion;

            if (!dependencyVersionTranslators.ContainsKey(dependencyType))
                throw new VersionParsingException($"Could not parse the version of the dependency type {dependencyType}.");

            return dependencyVersionTranslators[dependencyType].Translate(version);
        }

        private static IConnectionStringProvider ParseConnectionStringProvider(JsonNode node, IFilePathMapper pathMapper)
        {
            if (node.AsObject == null)
                return new StaticConnectionStringProvider(node.Value);

            JsonClass objClass = node.AsObject;
            if (!Enum.TryParse(node["locator"]?.Value, out SupportedConnectionStringLocators locator))
                throw new NAMEException($"The locator {node["locator"]?.Value} is not supported.", NAMEStatusLevel.Warn);
            IConnectionStringProvider provider = null;
            string key;
            switch (locator)
            {
#if NET45
                case SupportedConnectionStringLocators.ConnectionStrings:
                    key = node["key"]?.Value;
                    provider = new ConnectionStringsConnectionStringProvider(key);
                    break;
                case SupportedConnectionStringLocators.AppSettings:
                    key = node["key"]?.Value;
                    provider = new AppSettingsConnectionStringProvider(key);
                    break;
                case SupportedConnectionStringLocators.VSSettingsFile:
                    key = node["key"]?.Value;
                    string section = node["section"]?.Value;
                    if (string.IsNullOrEmpty(section))
                        throw new ArgumentNullException("section", "The section must be specified.");
                    provider = new VisualStudioSetingsFileConnectionStringProvider(section, key);
                    break;
#endif
                case SupportedConnectionStringLocators.JSONPath:
                    {
                        key = node["expression"]?.Value;
                        string file = node["file"]?.Value;
                        if (string.IsNullOrEmpty(file))
                            throw new ArgumentNullException("file", "The file must be specified.");
                        provider = new JsonPathConnectionStringProvider(pathMapper.MapPath(file), key);
                    }
                    break;
                case SupportedConnectionStringLocators.XPath:
                    {
                        key = node["expression"]?.Value;
                        string file = node["file"]?.Value;
                        if (string.IsNullOrEmpty(file))
                            throw new ArgumentNullException("file", "The file must be specified.");
                        provider = new XpathConnectionStringProvider(pathMapper.MapPath(file), key);
                        break;
                    }
                case SupportedConnectionStringLocators.EnvironmentVariable:
                    {
                        key = node["key"]?.Value;
                       
                        provider = new EnvironmentVariableConnectionStringProvider(key);
                    }
                    break;
                default:
                    throw new NAMEException($"The locator {locator.ToString()} is not supported.", NAMEStatusLevel.Warn);
            }

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException($"The connection string key/expression must be specified.");

            return provider;
        }

        private static IVersionResolver GetConnectedDependencyVersionResolver(SupportedDependencies dependencyType, IConnectionStringProvider connectionStringProvider, NAMESettings configuration, NAMEContext context)
        {
            switch (dependencyType)
            {
                case SupportedDependencies.MongoDb:
                    return new MongoDb.MongoDbVersionResolver(connectionStringProvider, configuration.DependencyConnectTimeout, configuration.DependencyReadWriteTimeout);
                case SupportedDependencies.RabbitMq:
                    return new RabbitMq.RabbitMqVersionResolver(connectionStringProvider, configuration.DependencyConnectTimeout, configuration.DependencyReadWriteTimeout);
                case SupportedDependencies.SqlServer:
                    return new SqlServer.SqlServerVersionResolver(connectionStringProvider, configuration.DependencyConnectTimeout, configuration.DependencyReadWriteTimeout);
                case SupportedDependencies.Service:
                    return new Service.ServiceVersionResolver(connectionStringProvider, context.ServiceDependencyCurrentNumberOfHops, configuration.ServiceDependencyMaxHops, configuration.DependencyConnectTimeout, configuration.DependencyReadWriteTimeout);
                case SupportedDependencies.Elasticsearch:
                    return new Elasticsearch.ElasticsearchVersionResolver(connectionStringProvider, configuration.DependencyConnectTimeout, configuration.DependencyReadWriteTimeout);
                default:
                    throw new NAMEException($"The dependency of type {dependencyType} is not supported as a connected dependency.", NAMEStatusLevel.Warn);
            }
        }

        /// <summary>
        /// Reads the NAME settings overrides.
        /// </summary>
        /// <param name="settingsFile">The settings file.</param>
        /// <param name="pathMapper">The path mapper.</param>
        /// <returns>
        /// Returns the <see cref="NAMESettings" />.
        /// </returns>
        public static NAMESettings ReadNAMESettingsOverrides(string settingsFile, IFilePathMapper pathMapper)
        {
            Guard.NotNull(settingsFile, nameof(settingsFile));

            var jsonContents = ReadJsonContents(pathMapper.MapPath(settingsFile));

            JsonNode rootNode = Json.Json.Parse(jsonContents);
            var overrideNode = rootNode["Overrides"];

            var settings = new NAMESettings();

            if (overrideNode == null)
                return settings;

            var registryEndpoints = overrideNode[nameof(settings.RegistryEndpoints)]?.AsArray;
            if (registryEndpoints != null)
            {
                settings.RegistryEndpoints = new string[registryEndpoints.Count];
                for (int i = 0; i < registryEndpoints.Count; i++)
                {
                    settings.RegistryEndpoints[i] = registryEndpoints[i].Value;
                }
            }

            var selfhostPortRangeFirst = overrideNode[nameof(settings.SelfHostPortRangeFirst)]?.AsInt;
            if (selfhostPortRangeFirst != null)
                settings.SelfHostPortRangeFirst = selfhostPortRangeFirst.Value;

            var selfhostPortRangeLast = overrideNode[nameof(settings.SelfHostPortRangeLast)]?.AsInt;
            if (selfhostPortRangeLast != null)
                settings.SelfHostPortRangeLast = selfhostPortRangeLast.Value;

            var serviceDependencyMaxHops = overrideNode[nameof(settings.ServiceDependencyMaxHops)]?.AsInt;
            if (serviceDependencyMaxHops != null)
                settings.ServiceDependencyMaxHops = serviceDependencyMaxHops.Value;

            var serviceDependencyShowConnectionString = overrideNode[nameof(settings.ConnectedDependencyShowConnectionString)]?.AsBool;
            if (serviceDependencyShowConnectionString != null)
                settings.ConnectedDependencyShowConnectionString = serviceDependencyShowConnectionString.Value;

            var dependencyConnectTimeout = overrideNode[nameof(settings.DependencyConnectTimeout)]?.AsInt;
            if (dependencyConnectTimeout != null)
                settings.DependencyConnectTimeout = dependencyConnectTimeout.Value;

            var dependencyReadWriteTimeout = overrideNode[nameof(settings.DependencyReadWriteTimeout)]?.AsInt;
            if (dependencyReadWriteTimeout != null)
                settings.DependencyReadWriteTimeout = dependencyReadWriteTimeout.Value;

            var registryReAnnounceFrequency = overrideNode[nameof(settings.RegistryReAnnounceFrequency)];
            if (registryReAnnounceFrequency != null && TimeSpan.TryParse(registryReAnnounceFrequency, out TimeSpan parsedAnnounceFreq))
                settings.RegistryReAnnounceFrequency = parsedAnnounceFreq;

            var registryPingFrequency = overrideNode[nameof(settings.RegistryPingFrequency)];
            if (registryPingFrequency != null && TimeSpan.TryParse(registryPingFrequency, out TimeSpan parsedPingFreq))
                settings.RegistryPingFrequency = parsedPingFreq;

            var runningMode = overrideNode[nameof(settings.RunningMode)];
            if (runningMode != null && Enum.TryParse<SupportedNAMEBehaviours>(runningMode.Value.ToString(), false, out var behaviour))
                settings.RunningMode = behaviour;

            var registryBootstrapRetryFrequency = overrideNode[nameof(settings.RegistryBootstrapRetryFrequency)];
            if (registryBootstrapRetryFrequency != null && TimeSpan.TryParse(registryBootstrapRetryFrequency, out TimeSpan bootstrapRetryFreq))
                settings.RegistryBootstrapRetryFrequency = bootstrapRetryFreq;

            var registryBootstrapConnectTimeout = overrideNode[nameof(settings.RegistryBootstrapTimeout)];
            if (registryBootstrapConnectTimeout != null && TimeSpan.TryParse(registryBootstrapConnectTimeout, out TimeSpan bootstrapConnectTimeout))
                settings.RegistryBootstrapTimeout = bootstrapConnectTimeout;

            return settings;
        }

    }
}
