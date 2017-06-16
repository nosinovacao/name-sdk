using NAME.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace NAME.WebApi
{
    internal class WebApiFilePathMapper : IFilePathMapper
    {
        public string MapPath(string filePath)
        {
            if (filePath == null)
                return null;

            string path = null;
            if (filePath.StartsWith("~") || filePath.StartsWith("\\") || filePath.StartsWith("/"))
                path = HostingEnvironment.MapPath(filePath);

            // path is null if this is a self host environment (e.g. OWIN).
            if (path == null)
            {
                var uriPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                path = Path.Combine(new Uri(uriPath).LocalPath, filePath);
            }

            return path;
        }
    }
}