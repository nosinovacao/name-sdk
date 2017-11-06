using System;
using NAME.SelfHost.Kestrel;
using System.Reflection;

namespace NAME.DummyConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Action<NAMEKestrelConfiguration> configBuilder = config =>
            {
                Assembly a = typeof(Program).GetTypeInfo().Assembly;
                config.APIName = a.GetName().Name;
                config.APIVersion = a.GetName().Version.ToString();
                config.LogHealthCheckToConsole = true;
                config.ThrowOnDependenciesFail = false;
            };

            using (var selfHost = NAMEServer.EnableName(configBuilder))
            {
                Console.WriteLine("Hello World!");
                Console.ReadKey();
            }
        }
    }
}
