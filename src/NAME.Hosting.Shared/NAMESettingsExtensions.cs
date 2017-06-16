using NAME.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.Hosting.Shared
{
    /// <summary>
    /// Provides extension methods for <see cref="NAMESettings"/>.
    /// </summary>
    public static class NAMESettingsExtensions
    {
        /// <summary>
        /// Determines if the manifest endpoint should be enabled considering the current <see cref="NAMESettings"/> state.
        /// </summary>
        /// <param name="nameSettings">The NAME settings.</param>
        /// <returns>Returns true if the endpoint should be enabled.</returns>
        public static bool IsManifestEndpointEnabled(this NAMESettings nameSettings)
        {
            return nameSettings.RunningMode != SupportedNAMEBehaviours.NAMEDisabled;
        }
    }
}
