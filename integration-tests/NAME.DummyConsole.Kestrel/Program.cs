using System;
using NAME.SelfHost.Kestrel;
using System.Reflection;
using System.Threading;

namespace NAME.DummyConsole
{
    class Program
    {
        private static readonly AutoResetEvent _closing = new AutoResetEvent(false);

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

                Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
                _closing.WaitOne();
            }
        }

        protected static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Exiting");
            _closing.Set();
        }
    }
}
