using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NAME.Utils
{
    /// <summary>
    /// Provides ways to fetch resourcess from the NAME assembly.
    /// </summary>
    public static class ResourcesFetcher
    {
        private const string MANIFEST_LINK_PLACEHOLDER = "[MANIFEST_LINK_PLACEHOLDER]";

        /// <summary>
        /// Gets the NAME UI HTML page.
        /// </summary>
        /// <param name="manifestUrl">The manifest URL.</param>
        /// <returns>Returns the HTML page for the NAME UI.</returns>
        public static async Task<string> GetNAMEUiAsync(string manifestUrl)
        {
            using (Stream s = typeof(ResourcesFetcher).GetTypeInfo().Assembly.GetManifestResourceStream("NAME.Resources.NAME_UI.html"))
            using (StreamReader reader = new StreamReader(s))
            {
                string tempHtml = await reader.ReadToEndAsync();
                return tempHtml.Replace(MANIFEST_LINK_PLACEHOLDER, manifestUrl);
            }
        }

        /// <summary>
        /// Gets the NAME UI HTML page.
        /// </summary>
        /// <param name="manifestUrl">The manifest URL.</param>
        /// <returns>Returns the HTML page for the NAME UI.</returns>
        public static string GetNAMEUi(string manifestUrl)
        {
            using (Stream s = typeof(ResourcesFetcher).GetTypeInfo().Assembly.GetManifestResourceStream("NAME.Resources.NAME_UI.html"))
            using (StreamReader reader = new StreamReader(s))
            {
                string tempHtml = reader.ReadToEnd();
                return tempHtml.Replace(MANIFEST_LINK_PLACEHOLDER, manifestUrl);
            }
        }
    }
}
