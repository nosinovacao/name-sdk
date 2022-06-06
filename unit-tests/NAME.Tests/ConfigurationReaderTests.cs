using NAME.ConnectionStrings;
using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Dependencies;
using NAME.Json;
using NAME.Tests.ConnectionStrings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NAME.Tests
{
    [TestCaseOrderer("NAME.Tests.PriorityOrderer", "NAME.Tests")]
    public class ConfigurationReaderTests : IClassFixture<CreateConfigurationFileFixture>
    {
        #region configuration
        public static string CONFIGURATION_FILE = Guid.NewGuid() + ".json";
        public const string CONFIGURATION_CONTENTS =
            @"{
                ""infrastructure_dependencies"": [
                    {
                        ""oneOf"": [
                            {
                                ""os_name"": ""Ubuntu"",
                                ""type"": ""OperatingSystem"",
                                ""min_version"": ""16.04"",
                                ""max_version"": ""14.04""
                            },
                            {
                                ""os_name"": ""Windows"",
                                ""type"": ""OperatingSystem"",
                                ""min_version"": ""WindowsServer2008R2"",
                                ""max_version"": ""*""
                            }
                        ]
                    },
                    {
                        ""type"": ""MongoDb"",
                        ""min_version"": ""2.6"",
                        ""max_version"": ""4.0"",
                        ""connection_string"": ""mongodb://some-mongodb-instance:27017/some-db""
                    },
                    {
                        ""type"": ""Elasticsearch"",
                        ""min_version"": ""2.6"",
                        ""max_version"": ""4.*"",
                        ""connection_string"": ""http://some-elasticsearch-instance:9200""
                    }"
#if NET462
                    + @",{
                        ""type"": ""RabbitMq"",
                        ""name"": ""rabbitmq"",
                        ""min_version"": ""2.0"",
                        ""max_version"": ""3.3"",
                        ""connection_string"": {
                            ""locator"": ""ConnectionStrings"",
                            ""key"": ""RabbitMQConnectionString""
                        }
                    }"
#endif
                + @"],
                ""service_dependencies"": [
                    {
                        ""name"": ""Some Service"",
                        ""min_version"": ""0.3"",
                        ""max_version"": ""*"",
                        ""connection_string"": ""http://some-services/api/""
                    },
                    {
                        ""name"": ""The other service"",
                        ""min_version"": ""1.0"",
                        ""max_version"": ""*"",
                        ""connection_string"": ""http://other-service/api/""
                    }
                ]
            }";
        #endregion
        
        [Fact]
        [Trait("TestCategory", "Unit")]
        public void ReadConfiguration_CallsMultipleTimes_ConnectionStringProviderOverride()
        {
            string fileContents = @"{
                ""infrastructure_dependencies"": [
                ],
                ""service_dependencies"": [
                    {
                        ""name"": ""Some Service0"",
                        ""min_version"": ""0.3"",
                        ""max_version"": ""*"",
                        ""connection_string"": {
                            ""unrecognizedString"": 0
                        }
                    },
                    {
                        ""name"": ""Some Service1"",
                        ""min_version"": ""0.3"",
                        ""max_version"": ""*"",
                        ""connection_string"": {
                            ""unrecognizedString"": 1
                        }
                    }
                ]
            }";
            string fileName = Guid.NewGuid() + ".json";
            File.WriteAllText(fileName, fileContents);
            try
            {
                int iterationsCount = 0;
                var settings = new NAMESettings()
                {
                    ConnectionStringProviderOverride = (node) =>
                    {
                        Assert.Equal(iterationsCount, node["unrecognizedString"].AsInt);
                        iterationsCount++;
                        return new StaticConnectionStringProvider(iterationsCount.ToString());
                    }
                };

                ParsedDependencies configuration = DependenciesReader.ReadDependencies(fileName, new DummyFilePathMapper(), settings, new NAMEContext());

                Assert.Equal(0, configuration.InfrastructureDependencies.Count());
                Assert.Equal(2, configuration.ServiceDependencies.Count());
                Assert.Equal(2, iterationsCount);

                var firstDependency = (ConnectedDependency)configuration.ServiceDependencies.First();
                var secondDependency = (ConnectedDependency)configuration.ServiceDependencies.Skip(1).First();

                Assert.IsType<StaticConnectionStringProvider>(firstDependency.ConnectionStringProvider);
                Assert.IsType<StaticConnectionStringProvider>(secondDependency.ConnectionStringProvider);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void ReadConfiguration_UsesConnectionStringProviderReturnedFromOverride()
        {
            string fileContents = @"{
                ""infrastructure_dependencies"": [
                    {
                        ""type"": ""RabbitMq"",
                        ""name"": ""rabbitmq"",
                        ""min_version"": ""2.0"",
                        ""max_version"": ""3.3"",
                        ""connection_string"": {
                            ""locator"": ""JSONPath"",
                            ""key"": ""shouldn't matter""
                        }
                    }  
                ],
                ""service_dependencies"": [
                ]
            }";
            string fileName = Guid.NewGuid() + ".json";
            File.WriteAllText(fileName, fileContents);
            try
            {
                int iterationsCount = 0;
                var settings = new NAMESettings()
                {
                    ConnectionStringProviderOverride = (node) =>
                    {
                        iterationsCount++;
                        return new StaticConnectionStringProvider(string.Empty);
                    }
                };

                ParsedDependencies configuration = DependenciesReader.ReadDependencies(fileName, new DummyFilePathMapper(), settings, new NAMEContext());

                Assert.Equal(1, configuration.InfrastructureDependencies.Count());
                Assert.Equal(0, configuration.ServiceDependencies.Count());
                Assert.Equal(1, iterationsCount);

                var firstDependency = (ConnectedDependency)configuration.InfrastructureDependencies.First();
                Assert.IsType<StaticConnectionStringProvider>(firstDependency.ConnectionStringProvider);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void ReadConfiguration_UsesStaticConnectionStringProvider()
        {
            string fileContents = @"{
                ""infrastructure_dependencies"": [
                    {
                        ""type"": ""RabbitMq"",
                        ""name"": ""rabbitmq"",
                        ""min_version"": ""2.0"",
                        ""max_version"": ""3.3"",
                        ""connection_string"": ""some-connection-string""
                    }  
                ],
                ""service_dependencies"": [
                ]
            }";
            string fileName = Guid.NewGuid() + ".json";
            File.WriteAllText(fileName, fileContents);
            try
            {
                int iterationsCount = 0;
                var settings = new NAMESettings()
                {
                    ConnectionStringProviderOverride = (node) =>
                     {
                         iterationsCount++;
                         return null;
                     }
                };

                ParsedDependencies configuration = DependenciesReader.ReadDependencies(fileName, new DummyFilePathMapper(), settings, new NAMEContext());

                Assert.Equal(1, configuration.InfrastructureDependencies.Count());
                Assert.Equal(0, configuration.ServiceDependencies.Count());
                Assert.Equal(0, iterationsCount);

                var firstDependency = (ConnectedDependency)configuration.InfrastructureDependencies.First();
                Assert.IsType<StaticConnectionStringProvider>(firstDependency.ConnectionStringProvider);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void ReadConfiguration_OperatingSystemDependency_SkipsOverride()
        {
            string fileContents = @"{
                ""infrastructure_dependencies"": [
                    {
                        ""os_name"": ""Ubuntu"",
                        ""type"": ""OperatingSystem"",
                        ""min_version"": ""16.04"",
                        ""max_version"": ""14.04""
                    }
                ],
                ""service_dependencies"": [
                ]
            }";
            string fileName = Guid.NewGuid() + ".json";
            File.WriteAllText(fileName, fileContents);
            try
            {
                int iterationsCount = 0;
                var settings = new NAMESettings()
                {
                    ConnectionStringProviderOverride = (node) =>
                    {
                        iterationsCount++;
                        return null;
                    }
                };

                ParsedDependencies configuration = DependenciesReader.ReadDependencies(fileName, new DummyFilePathMapper(), settings, new NAMEContext());

                Assert.Equal(1, configuration.InfrastructureDependencies.Count());
                Assert.Equal(0, configuration.ServiceDependencies.Count());
                Assert.Equal(0, iterationsCount);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void ReadConfiguration_EmptyArrays()
        {
            string fileContents = @"{
                ""infrastructure_dependencies"": [
                ],
                ""service_dependencies"": [
                ]
            }";
            string fileName = Guid.NewGuid() + ".json";
            File.WriteAllText(fileName, fileContents);
            try
            {
                ParsedDependencies configuration = DependenciesReader.ReadDependencies(fileName, new DummyFilePathMapper(), new NAMESettings(), new NAMEContext());

                Assert.Equal(0, configuration.InfrastructureDependencies.Count());
                Assert.Equal(0, configuration.ServiceDependencies.Count());
            }
            finally
            {
                File.Delete(fileName);
            }
        }


        [Fact]
        [Trait("TestCategory", "Unit")]
        public void ReadConfiguration_FileAllCommented()
        {
            string fileContents = @"//{
                //""$schema"": ""./config-manifest.schema.json"",
                //""infrastructure_dependencies"": [
                //],
                //""service_dependencies"": [
                //]
            //}";
            string fileName = Guid.NewGuid() + ".json";
            File.WriteAllText(fileName, fileContents);
            try
            {
                ParsedDependencies configuration = DependenciesReader.ReadDependencies(fileName, new DummyFilePathMapper(), new NAMESettings(), new NAMEContext());

                Assert.Equal(0, configuration.InfrastructureDependencies.Count());
                Assert.Equal(0, configuration.ServiceDependencies.Count());
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void ReadConfiguration_WithSomeComments()
        {
            string fileContents = @"{
                ""$schema"": ""./config-manifest.schema.json"",
                ""infrastructure_dependencies"": [
                    //Comment this yeah!
                ],
                ""service_dependencies"": [
                    //Comment this yeah!
                ]
            //}";
            string fileName = Guid.NewGuid() + ".json";
            File.WriteAllText(fileName, fileContents);
            try
            {
                ParsedDependencies configuration = DependenciesReader.ReadDependencies(fileName, new DummyFilePathMapper(), new NAMESettings(), new NAMEContext());

                Assert.Equal(0, configuration.InfrastructureDependencies.Count());
                Assert.Equal(0, configuration.ServiceDependencies.Count());
            }
            finally
            {
                File.Delete(fileName);
            }
        }


        [Fact]
        [Trait("TestCategory", "Unit")]
        public void ReadConfiguration_WithTabulationComments()
        {
            string fileContents = @"{
                ""$schema"": ""./config-manifest.schema.json"",
            	""infrastructure_dependencies"": [
                	//Comment this yeah!
                ],
                ""service_dependencies"": [
                    //Comment this yeah!
                ]
            //}";
            string fileName = Guid.NewGuid() + ".json";
            File.WriteAllText(fileName, fileContents);
            try
            {
                ParsedDependencies configuration = DependenciesReader.ReadDependencies(fileName, new DummyFilePathMapper(), new NAMESettings(), new NAMEContext());

                Assert.Equal(0, configuration.InfrastructureDependencies.Count());
                Assert.Equal(0, configuration.ServiceDependencies.Count());
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void ReadConfiguration_WithAllOverrides()
        {
            string fileContents = @"{
                ""$schema"": ""./config-manifest.schema.json"",
                ""Overrides"": {
                    ""RunningMode"": ""NAMEDisabled"",
                    ""RegistryEndpoints"": [
                          ""http://name:80/api"",
                          ""http://name2:80/api""
                    ],
                    ""SelfHostPortRangeFirst"": 1,
                    ""SelfHostPortRangeLast"": 10,
                    ""ServiceDependencyMaxHops"": 2,
                    ""ConnectedDependencyShowConnectionString"": false,
                    ""DependencyConnectTimeout"": 429496721,
                    ""DependencyReadWriteTimeout"": 429496722,
                    ""RegistryBootstrapRetryFrequency"": ""00.02:00:00"",
                    ""RegistryBootstrapTimeout"": ""00.00:00:31""
                },
                ""infrastructure_dependencies"": [
                ],
                ""service_dependencies"": [
                ]
            }";

            string fileName = Guid.NewGuid() + ".json";
            File.WriteAllText(fileName, fileContents);
            try
            {
                NAMESettings settings = DependenciesReader.ReadNAMESettingsOverrides(fileName, new DummyFilePathMapper());
                ParsedDependencies configuration = DependenciesReader.ReadDependencies(fileName, new DummyFilePathMapper(), settings, new NAMEContext());

                Assert.Equal(0, configuration.InfrastructureDependencies.Count());
                Assert.Equal(0, configuration.ServiceDependencies.Count());

                Assert.Equal(SupportedNAMEBehaviours.NAMEDisabled, settings.RunningMode);
                Assert.Equal(new[] { "http://name:80/api", "http://name2:80/api" }, settings.RegistryEndpoints);
                Assert.Equal(1, settings.SelfHostPortRangeFirst);
                Assert.Equal(10, settings.SelfHostPortRangeLast);
                Assert.Equal(2, settings.ServiceDependencyMaxHops);
                Assert.False(settings.ConnectedDependencyShowConnectionString);
                Assert.Equal(429496721, settings.DependencyConnectTimeout);
                Assert.Equal(429496722, settings.DependencyReadWriteTimeout);
                Assert.Equal(TimeSpan.FromHours(2), settings.RegistryBootstrapRetryFrequency);
                Assert.Equal(TimeSpan.FromSeconds(31), settings.RegistryBootstrapTimeout);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        [TestPriority(2)]
        public void ReadConfiguration()
        {
#if NET462
            System.Configuration.ConfigurationManager.ConnectionStrings.SetWritable().Add(new System.Configuration.ConnectionStringSettings("RabbitMQConnectionString", "ConnString"));
#endif
            int overrideCallsCount = 0;
            var settings = new NAMESettings()
            {
                ConnectionStringProviderOverride = (node) =>
                {
                    overrideCallsCount = overrideCallsCount + 1;
                    return null;
                }
            };

            ParsedDependencies configuration = DependenciesReader.ReadDependencies(CONFIGURATION_FILE, new DummyFilePathMapper(), settings, new NAMEContext());

#if NET462
            Assert.Equal(4, configuration.InfrastructureDependencies.Count());
            Assert.Equal(2, configuration.ServiceDependencies.Count());
            Assert.Equal(1, overrideCallsCount);
#else
            Assert.Equal(3, configuration.InfrastructureDependencies.Count());
            Assert.Equal(2, configuration.ServiceDependencies.Count());
            Assert.Equal(0, overrideCallsCount);
#endif
            var elasticsearchDependency = configuration.InfrastructureDependencies.OfType<VersionedDependency>().Single(d => d.Type == SupportedDependencies.Elasticsearch);
            var castedMaxVersion = Assert.IsAssignableFrom<WildcardDependencyVersion>(elasticsearchDependency.MaximumVersion);
            Assert.False(castedMaxVersion.IsMajorWildcard);
            Assert.True(castedMaxVersion.IsMinorWildcard);
        }

#if NET462
        [Fact]
        [Trait("TestCategory", "Unit")]
        [TestPriority(1)]
        public void ReadConfiguration_ConnectionStringNotFound()
        {
            System.Configuration.ConfigurationManager.ConnectionStrings.SetWritable().Remove("RabbitMQConnectionString");
            DependenciesReader.ReadDependencies(CONFIGURATION_FILE, new DummyFilePathMapper(), new NAMESettings(), new NAMEContext());
        }
#endif
    }

    public class CreateConfigurationFileFixture : IDisposable
    {
        public CreateConfigurationFileFixture()
        {
            if (File.Exists(ConfigurationReaderTests.CONFIGURATION_FILE))
                File.Delete(ConfigurationReaderTests.CONFIGURATION_FILE);
            File.WriteAllText(ConfigurationReaderTests.CONFIGURATION_FILE, ConfigurationReaderTests.CONFIGURATION_CONTENTS);
        }

        public void Dispose()
        {
            File.Delete(ConfigurationReaderTests.CONFIGURATION_FILE);
        }

    }
}
