using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NAME.IntegrationTests
{
    public class ManifestGeneratorTests : IClassFixture<ManifestGeneratorFileFixture>
    {
        public static string CONFIGURATION_FILE = Guid.NewGuid() + ".json";
        public static string CONFIGURATION_CONTENTS = @"{
                ""$schema"": ""./config-manifest.schema.json"",
                ""infrastructure_dependencies"": [
                    {
                        ""name"": ""Mongo"",
                        ""type"": ""MongoDb"",
                        ""min_version"": ""2.6.0"",
                        ""max_version"": ""*"",
                        ""connection_string"": ""mongodb://" + Constants.SpecificMongoHostname + @":27017/nPVR_Dev_TST""
                    }
                ],
                ""service_dependencies"": [
                ]
            }";

        public static string WRONG_VERSION_CONFIGURATION_FILE = Guid.NewGuid() + ".json";
        public static string WRONG_VERSION_CONFIGURATION_CONTENTS = @"{
                ""$schema"": ""./config-manifest.schema.json"",
                ""infrastructure_dependencies"": [
                    {
                        ""name"": ""Mongo"",
                        ""type"": ""MongoDb"",
                        ""min_version"": ""1.0.0"",
                        ""max_version"": ""2.*"",
                        ""connection_string"": ""mongodb://" + Constants.SpecificMongoHostname + @":27017/nPVR_Dev_TST""
                    }
                ],
                ""service_dependencies"": [
                ]
            }";

        public static string SERVICE_WITHOUT_NAME_CONFIGURATION_FILE = Guid.NewGuid() + ".json";
        public static string SERVICE_WITHOUT_NAME_CONFIGURATION_CONTENTS = @"{
                ""$schema"": ""./config-manifest.schema.json"",
                ""infrastructure_dependencies"": [
                ],
                ""service_dependencies"": [
                    {
                        ""name"": ""dummy"",
                        ""min_version"": ""1.0.0"",
                        ""max_version"": ""2.*"",
                        ""connection_string"": ""http://" + Constants.SpecificServiceHostname + @":5000/endpoint/before/name/middleware""
                    }
                ]
            }";


        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GenerateJson()
        {
            var runningVersion = typeof(ManifestGeneratorTests).GetTypeInfo().Assembly.GetName().Version.ToString(3);

            string expectedManifest = @"{
                    ""nameVersion"": """ + NAME.Core.Constants.NAME_ASSEMBLY_VERSION + @""",
                    ""name"":""NAME.Tests"",
                    ""version"":""" + runningVersion + @""",
                    ""infrastructure_dependencies"":[
                        {
                            ""name"":""Mongo"",
                            ""version"":""" + Constants.SpecificMongoVersion + @""",
                            ""min_version"":""2.6.0"",
                            ""max_version"":""*"",
                            ""status"":""Ok"",
                            ""value"":""mongodb://" + Constants.SpecificMongoHostname + @":27017/nPVR_Dev_TST""
                        }
                    ],
                    ""service_dependencies"":[
                    ]
                }";
            ParsedDependencies dependencies = DependenciesReader.ReadDependencies(CONFIGURATION_FILE, new DummyFilePathMapper(), new Core.NAMESettings(), new Core.NAMEContext());
            string manifest = await ManifestGenerator.GenerateJson("NAME.Tests", runningVersion, dependencies);

            Assert.Equal(
                            expectedManifest.Replace(" ", string.Empty).Replace("\t", "").Replace("\r", "").Replace("\n", ""),
                                        manifest.Replace(" ", string.Empty).Replace("\t", "").Replace("\r", "").Replace("\n", ""));
        }

        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GenerateJson_WrongVersion()
        {
            var runningVersion = typeof(ManifestGeneratorTests).GetTypeInfo().Assembly.GetName().Version.ToString(3);

            string expectedManifest = @"{
                    ""nameVersion"": """ + NAME.Core.Constants.NAME_ASSEMBLY_VERSION + @""",
                    ""name"":""NAME.Tests"",
                    ""version"":""" + runningVersion + @""",
                    ""infrastructure_dependencies"":[
                        {
                            ""name"":""Mongo"",
                            ""version"":""" + Constants.SpecificMongoVersion + @""",
                            ""min_version"":""1.0.0"",
                            ""max_version"":""2.*"",
                            ""status"":""Error"",
                            ""value"":""mongodb://" + Constants.SpecificMongoHostname + @":27017/nPVR_Dev_TST""
                        }
                    ],
                    ""service_dependencies"":[
                    ]
                }";
            ParsedDependencies dependencies = DependenciesReader.ReadDependencies(WRONG_VERSION_CONFIGURATION_FILE, new DummyFilePathMapper(), new Core.NAMESettings(), new Core.NAMEContext());
            string manifest = await ManifestGenerator.GenerateJson("NAME.Tests", runningVersion, dependencies);

            Assert.Equal(
                            expectedManifest.Replace(" ", string.Empty).Replace("\t", "").Replace("\r", "").Replace("\n", ""),
                                        manifest.Replace(" ", string.Empty).Replace("\t", "").Replace("\r", "").Replace("\n", ""));
        }

        [Fact]
        [Trait("TestCategory", "Integration")]
        public async Task GenerateJson_ServiceWithoutNAME()
        {
            var runningVersion = typeof(ManifestGeneratorTests).GetTypeInfo().Assembly.GetName().Version.ToString(3);

            string expectedManifest = @"{
                    ""nameVersion"": """ + NAME.Core.Constants.NAME_ASSEMBLY_VERSION + @""",
                    ""name"":""NAME.Tests"",
                    ""version"":""" + runningVersion + @""",
                    ""infrastructure_dependencies"":[
                    ],
                    ""service_dependencies"":[
                        {
                            ""name"": ""dummy"",
                            ""error"": ""Dependency does not have NAME installed!"",
                            ""status"":""" + NAMEStatusLevel.Warn.ToString() + @""",
                            ""min_version"": ""1.0.0"",
                            ""max_version"": ""2.*"",
                            ""connection_string"": ""http://" + Constants.SpecificServiceHostname + @":5000/endpoint/before/name/middleware""
                        }
                    ]
                }";
            ParsedDependencies dependencies = DependenciesReader.ReadDependencies(SERVICE_WITHOUT_NAME_CONFIGURATION_FILE, new DummyFilePathMapper(), new Core.NAMESettings(), new Core.NAMEContext());
            string manifest = await ManifestGenerator.GenerateJson("NAME.Tests", runningVersion, dependencies);

            Assert.Equal(
                            expectedManifest.Replace(" ", string.Empty).Replace("\t", "").Replace("\r", "").Replace("\n", ""),
                                        manifest.Replace(" ", string.Empty).Replace("\t", "").Replace("\r", "").Replace("\n", ""));
        }
    }


    public class ManifestGeneratorFileFixture : IDisposable
    {
        public ManifestGeneratorFileFixture()
        {
            if (File.Exists(ManifestGeneratorTests.CONFIGURATION_FILE))
                File.Delete(ManifestGeneratorTests.CONFIGURATION_FILE);
            File.WriteAllText(ManifestGeneratorTests.CONFIGURATION_FILE, ManifestGeneratorTests.CONFIGURATION_CONTENTS);

            if (File.Exists(ManifestGeneratorTests.WRONG_VERSION_CONFIGURATION_FILE))
                File.Delete(ManifestGeneratorTests.WRONG_VERSION_CONFIGURATION_FILE);
            File.WriteAllText(ManifestGeneratorTests.WRONG_VERSION_CONFIGURATION_FILE, ManifestGeneratorTests.WRONG_VERSION_CONFIGURATION_CONTENTS);

            if (File.Exists(ManifestGeneratorTests.SERVICE_WITHOUT_NAME_CONFIGURATION_FILE))
                File.Delete(ManifestGeneratorTests.SERVICE_WITHOUT_NAME_CONFIGURATION_FILE);
            File.WriteAllText(ManifestGeneratorTests.SERVICE_WITHOUT_NAME_CONFIGURATION_FILE, ManifestGeneratorTests.SERVICE_WITHOUT_NAME_CONFIGURATION_CONTENTS);
        }

        public void Dispose()
        {
            File.Delete(ManifestGeneratorTests.CONFIGURATION_FILE);
            File.Delete(ManifestGeneratorTests.WRONG_VERSION_CONFIGURATION_FILE);
            File.Delete(ManifestGeneratorTests.SERVICE_WITHOUT_NAME_CONFIGURATION_FILE);
        }

    }
}
