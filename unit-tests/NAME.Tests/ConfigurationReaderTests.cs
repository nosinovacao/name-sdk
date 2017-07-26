using NAME.Core;
using NAME.Core.Exceptions;
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
                    }"
#if NET452
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
#if NET452
            System.Configuration.ConfigurationManager.ConnectionStrings.SetWritable().Add(new System.Configuration.ConnectionStringSettings("RabbitMQConnectionString", "ConnString"));
#endif
            ParsedDependencies configuration = DependenciesReader.ReadDependencies(CONFIGURATION_FILE, new DummyFilePathMapper(), new NAMESettings(), new NAMEContext());

#if NET452
            Assert.Equal(3, configuration.InfrastructureDependencies.Count());
            Assert.Equal(2, configuration.ServiceDependencies.Count());
#else
            Assert.Equal(2, configuration.InfrastructureDependencies.Count());
            Assert.Equal(2, configuration.ServiceDependencies.Count());
#endif
        }

#if NET452
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
