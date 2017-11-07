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
    }


    public class ManifestGeneratorFileFixture : IDisposable
    {
        public ManifestGeneratorFileFixture()
        {
            if (File.Exists(ManifestGeneratorTests.CONFIGURATION_FILE))
                File.Delete(ManifestGeneratorTests.CONFIGURATION_FILE);
            File.WriteAllText(ManifestGeneratorTests.CONFIGURATION_FILE, ManifestGeneratorTests.CONFIGURATION_CONTENTS);
        }

        public void Dispose()
        {
            File.Delete(ManifestGeneratorTests.CONFIGURATION_FILE);
        }

    }
}
