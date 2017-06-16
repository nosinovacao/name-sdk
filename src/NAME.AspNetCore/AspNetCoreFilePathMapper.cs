using Microsoft.AspNetCore.Hosting;
using NAME.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.AspNetCore
{
    /// <summary>
    /// Provides a path mapper for Asp.Net Core
    /// </summary>
    /// <seealso cref="NAME.Core.IFilePathMapper" />
    public class AspNetCoreFilePathMapper : IFilePathMapper
    {
        private IHostingEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetCoreFilePathMapper"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        public AspNetCoreFilePathMapper(IHostingEnvironment environment)
        {
            this.environment = environment;
        }

        /// <summary>
        /// Maps the specified path to an absolute path.
        /// </summary>
        /// <param name="filePath">The relative file path.</param>
        /// <returns>
        /// Returns the absolute file path.
        /// </returns>
        public string MapPath(string filePath)
        {
            return System.IO.Path.Combine(this.environment.ContentRootPath ?? string.Empty, filePath);
        }
    }
}
