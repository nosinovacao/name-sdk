using NAME.Core;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NAME.Tests
{
    public class ManifestGeneratorTests
    {
        public static string CONFIGURATION_CONTENTS = @"{
                ""$schema"": ""./config-manifest.schema.json"",
                ""infrastructure_dependencies"": [
                ],
                ""service_dependencies"": [
                ]
            }";


        [Fact]
        [Trait("TestCategory", "Unit")]
        public async Task GenerateJson()
        {
            string fileName = Guid.NewGuid().ToString() + ".json";
            try
            {
                File.WriteAllText(fileName, CONFIGURATION_CONTENTS);
                string expectedManifest = @"{
                    ""nameVersion"": """ + Constants.NAME_ASSEMBLY_VERSION + @""",
                    ""name"":""NAME.Tests"",
                    ""version"":""1.0.0"",
                    ""infrastructure_dependencies"":[
                    ],
                    ""service_dependencies"":[
                    ]
                }";
                string appName = "NAME.Tests";
                string appVersion = "123.1.2";

                ParsedDependencies dependencies = DependenciesReader.ReadDependencies(fileName, new DummyFilePathMapper(), new NAMESettings(), new NAMEContext());
                string manifest = await ManifestGenerator.GenerateJson(appName, appVersion, dependencies);

                var manifestObject = (JObject)JsonConvert.DeserializeObject(manifest);

                Assert.Equal(Constants.NAME_ASSEMBLY_VERSION, manifestObject["nameVersion"]);
                Assert.Equal(appName, manifestObject["name"]);
                Assert.Equal(appVersion, manifestObject["version"]);
            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }

}
