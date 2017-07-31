using System.Reflection;
using System.Web.Http;
using $rootnamespace$;
using WebActivatorEx;
using NAME;
using NAME.WebApi;

[assembly: PreApplicationStartMethod(typeof(NAMEConfig), "Register")]

namespace $rootnamespace$
{
    
    /// <summary>
    /// Provides configuration for NAME.
    /// </summary>
    public class NAMEConfig
    {
        /// <summary>
        /// Registers the NAME plugin
        /// </summary>
        public static void Register()
        {
            //Enables NAME and checks that all the declared dependencies match the dependencies.json file.
            var assembly = typeof(NAMEConfig).Assembly;
            GlobalConfiguration.Configuration
                .EnableNAME(config =>
                {
                    // Use this property to tset the dependencies file.
                    // By default it has the ~/dependencies.json value.
                    //
                    //config.DependenciesFilePath = "~/dependencies.json";

                    // Use this property to set the name provided in the Manifest. 
                    //
                    config.APIName = assembly.GetName().Name;

                    // Use this property to set the version provided in the Manifest
                    //
                    config.APIVersion = assembly.GetName().Version.ToString();

                    // Use this property to set a prefix for the URI (before the /manifest), by default the prefix is empty.
                    //
                    //config.ManifestUriPrefix = "api/";

                    // Uncomment the following line to interrupt the Web Api startup if any of the dependencies fail.
                    // Warning: This will execute on Application recycle which may cause unexpected behaviours.
                    //
                    //config.ThrowOnDependenciesFail = true;
                });
        }
    }
}
