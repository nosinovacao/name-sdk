using NAME.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAME.SelfHost.Shared
{
    /// <summary>
    /// Provides a file path mapper for static paths.
    /// </summary>
    /// <seealso cref="NAME.Core.IFilePathMapper" />
    public class StaticFilePathMapper : IFilePathMapper
    {
        /// <summary>
        /// Maps the specified path to an absolute path.
        /// </summary>
        /// <param name="filePath">The relative file path.</param>
        /// <returns>
        /// Returns the absolute file path.
        /// </returns>
        public string MapPath(string filePath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), filePath);
        }
    }
}
