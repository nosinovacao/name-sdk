
using NAME.Core.Exceptions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

namespace NAME.Hosting.Shared.Tests
{
    public class DependenciesUtilsTests
    {

        [Fact]
        public void ReadAndLogDependencies_FailDoNotThrow()
        {
            string file = Guid.NewGuid().ToString();
            string contents =
                @"{
                ""infrastructure_dependencies"": [
                ],
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

            File.WriteAllText(file, contents);
            try
            {
                var config = new NAMEBaseConfiguration
                {
                    ThrowOnDependenciesFail = false,
                    DependenciesFilePath = file,
                    APIName = "hey",
                    APIVersion = "1.0.0"
                };

                var dependencies = DependenciesUtils.ReadAndLogDependencies(config, true, new DummyFilePathMapper(), out var settings);

                Assert.Empty(dependencies.InfrastructureDependencies);
                Assert.Equal(2, dependencies.ServiceDependencies.Count());
            }
            finally
            {
                File.Delete(file);
            }
        }

        [Fact]
        public void ReadAndLogDependencies_ValidDependencies()
        {
            string file = Guid.NewGuid().ToString();
            string contents =
                @"{
                ""infrastructure_dependencies"": [
                ],
                ""service_dependencies"": [
                ]
            }";

            File.WriteAllText(file, contents);
            try
            {
                var config = new NAMEBaseConfiguration
                {
                    ThrowOnDependenciesFail = true,
                    DependenciesFilePath = file,
                    APIName = "hey",
                    APIVersion = "1.0.0"
                };
                var dependencies = DependenciesUtils.ReadAndLogDependencies(config, true, new DummyFilePathMapper(), out var settings);
                Assert.Equal(0, dependencies.InfrastructureDependencies.Count());
                Assert.Equal(0, dependencies.ServiceDependencies.Count());
            }
            finally
            {
                File.Delete(file);
            }
        }

        [Fact]
        public void ReadAndLogDependencies_FailThrow()
        {
            string file = Guid.NewGuid().ToString();
            string contents =
                @"{
                ""infrastructure_dependencies"": [
                ],
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

            File.WriteAllText(file, contents);
            try
            {
                var config = new NAMEBaseConfiguration
                {
                    ThrowOnDependenciesFail = true,
                    DependenciesFilePath = file,
                    APIName = "hey",
                    APIVersion = "1.0.0"
                };
                Assert.Throws<DependenciesCheckException>(() =>
                {
                    DependenciesUtils.ReadAndLogDependencies(config, true, new DummyFilePathMapper(), out var settings);
                });
            }
            finally
            {
                File.Delete(file);
            }
        }

        [Fact]
        public void ReadAndLogDependencies_FailAsyncWhenThrowsEqualsFalse()
        {
            string file = Guid.NewGuid().ToString();
            string contents =
                @"{
                ""infrastructure_dependencies"": [
                ],
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

            File.WriteAllText(file, contents);
            try
            {
                var configThrows = new NAMEBaseConfiguration
                {
                    ThrowOnDependenciesFail = true,
                    DependenciesFilePath = file,
                    APIName = "hey",
                    APIVersion = "1.0.0"
                };

                var configDoesNotThrow = new NAMEBaseConfiguration
                {
                    ThrowOnDependenciesFail = false,
                    DependenciesFilePath = file,
                    APIName = "hey",
                    APIVersion = "1.0.0"
                };

                Stopwatch sw = new Stopwatch();
                sw.Start();
                Assert.Throws<DependenciesCheckException>(() =>
                {
                    DependenciesUtils.ReadAndLogDependencies(configThrows, true, new DummyFilePathMapper(), out var settings1);
                });
                sw.Stop();
                var throwsTimeTaken = sw.Elapsed;
                sw.Restart();
                var dependencies = DependenciesUtils.ReadAndLogDependencies(configDoesNotThrow, true, new DummyFilePathMapper(), out var settings2);
                sw.Stop();
                var doesNotThrowTimeTaken = sw.Elapsed;

                Assert.Empty(dependencies.InfrastructureDependencies);
                Assert.Equal(2, dependencies.ServiceDependencies.Count());
                Assert.True(throwsTimeTaken > doesNotThrowTimeTaken, $"When {nameof(NAMEBaseConfiguration.ThrowOnDependenciesFail)} is disabled, it should not take more time to complete. Should run asynchronously.");
            }
            finally
            {
                File.Delete(file);
            }
        }
    }
}
