using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NAME.Core
{
    /// <summary>
    /// Provides constants for NAME.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Status code for when the maximum number of hops for a service resolver is reached.
        /// Changing this value WILL break backwards compatibility
        /// </summary>
        public const int SERVICE_HOPS_ERROR_STATUS_CODE = 508;

        /// <summary>
        /// The hop count header name
        /// Changing this value WILL break backwards compatibility
        /// </summary>
        public const string HOP_COUNT_HEADER_NAME = "X-NAME-Hop-Count";

        /// <summary>
        /// The manifest endpoint header name.
        /// Changing this value WILL break backwards compatibility
        /// </summary>
        public const string MANIFEST_ENDPOINT_HEADER_NAME = "X-NAME-Manifest-Endpoint";

        /// <summary>
        /// The manifest endpoint path.
        /// Changing this value WILL break backwards compatibility
        /// </summary>
        public const string MANIFEST_ENDPOINT = "/manifest";

        /// <summary>
        /// The manifest UI endpoint path.
        /// Changing this value WILL break backwards compatibility
        /// </summary>
        public const string MANIFEST_UI_ENDPOINT = "/manifest/ui";

        /// <summary>
        /// Gets the assembly version of NAME.
        /// </summary>
        /// <value>
        /// The assembly version of NAME.
        /// </value>
        public static string NAME_ASSEMBLY_VERSION => typeof(Constants).GetTypeInfo().Assembly.GetName().Version.ToString(3);

        /// <summary>
        /// Gets the NAME Registry supported protocol versions.
        /// </summary>
        /// <value>
        /// The registry supported protocol versions.
        /// </value>
        public static uint[] REGISTRY_SUPPORTED_PROTOCOL_VERSIONS
        {
            get
            {
                return new[] { 1u };
            }
        }
    }
}
