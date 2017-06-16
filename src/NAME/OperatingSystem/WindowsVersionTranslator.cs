using NAME.Core;
using NAME.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NAME.OperatingSystem
{
    /// <summary>
    /// Provides a mechanism to translate between <see cref="WindowsVersions" /> and <see cref="OperatingSystemDependencyVersion" />.
    /// </summary>
    ///// <seealso cref="IVersionTranslator{TVersion}" />
    public class WindowsVersionTranslator : IVersionTranslator
    {
        /// <summary>
        /// Translates the specified <see cref="OperatingSystemDependencyVersion" /> to an object of type <see cref="WindowsVersions" />.
        /// </summary>
        /// <param name="source">The source <see cref="OperatingSystemDependencyVersion" />.</param>
        /// <returns>
        /// Returns an instance of type <see cref="WindowsVersions" />.
        /// </returns>
        /// <exception cref="VersionNotRecognizedException">Happens when the provided <see cref="OperatingSystemDependencyVersion"/> is not supported.</exception>
        public string Translate(DependencyVersion source)
        {
            //if (source.OperatingSystem != "windows")
            //    throw new VersionNotRecognizedException(source.ToString(), $"Windows: Cannot translate the version ({source.ToString()}) to a WindowsVersions!");

            if (source.Major == 6)
            {
                switch (source.Minor)
                {
                    case 0:
                        return WindowsVersions.WindowsServer2008.ToString();
                    case 1:
                        return WindowsVersions.WindowsServer2008R2.ToString();
                    case 2:
                        return WindowsVersions.WindowsServer2012.ToString();
                    case 3:
                        return WindowsVersions.WindowsServer2012R2.ToString();
                }
            }
            else if (source.Major == 5)
            {
                if (source.Minor == 1)
                    return WindowsVersions.WindowsXP.ToString();
                else if (source.Minor == 2)
                    return WindowsVersions.WindowsServer2003.ToString();
            }
            else if (source.Major == 10)
            {
                if (source.Minor == 0)
                    return WindowsVersions.WindowsServer2016.ToString();
            }

            throw new VersionNotRecognizedException(source.ToString(), $"Windows: Version ({source.ToString()}) not supported!");
        }

        /// <summary>
        /// Translates the specified <see cref="WindowsVersions" /> to a <see cref="OperatingSystemDependencyVersion" />.
        /// </summary>
        /// <param name="source">The source <see cref="WindowsVersions" />.</param>
        /// <returns>
        /// Returns a <see cref="OperatingSystemDependencyVersion" /> that identifies the specified source.
        /// </returns>
        /// <exception cref="VersionNotRecognizedException">Happens when the provided <see cref="WindowsVersions"/> is not supported.</exception>
        public DependencyVersion Translate(string source)
        {
            if (source == WindowsVersions.WindowsXP.ToString())
                return new OperatingSystemDependencyVersion("windows", 5, 1);
            if (source == WindowsVersions.WindowsXPProfessionalx64.ToString())
                return new OperatingSystemDependencyVersion("windows", 5, 2);
            if (source == WindowsVersions.WindowsServer2003.ToString())
                return new OperatingSystemDependencyVersion("windows", 5, 2);
            if (source == WindowsVersions.WindowsServer2003R2.ToString())
                return new OperatingSystemDependencyVersion("windows", 5, 2);
            if (source == WindowsVersions.WindowsVista.ToString())
                return new OperatingSystemDependencyVersion("windows", 6, 0);
            if (source == WindowsVersions.WindowsServer2008.ToString())
                return new OperatingSystemDependencyVersion("windows", 6, 0);
            if (source == WindowsVersions.WindowsServer2008R2.ToString())
                return new OperatingSystemDependencyVersion("windows", 6, 1);
            if (source == WindowsVersions.Windows7.ToString())
                return new OperatingSystemDependencyVersion("windows", 6, 1);
            if (source == WindowsVersions.WindowsServer2012.ToString())
                return new OperatingSystemDependencyVersion("windows", 6, 2);
            if (source == WindowsVersions.Windows8.ToString())
                return new OperatingSystemDependencyVersion("windows", 6, 2);
            if (source == WindowsVersions.WindowsServer2012R2.ToString())
                return new OperatingSystemDependencyVersion("windows", 6, 3);
            if (source == WindowsVersions.Windows81.ToString())
                return new OperatingSystemDependencyVersion("windows", 6, 3);
            if (source == WindowsVersions.WindowsServer2016.ToString())
                return new OperatingSystemDependencyVersion("windows", 10, 0);
            if (source == WindowsVersions.Windows10.ToString())
                return new OperatingSystemDependencyVersion("windows", 10, 0);

            throw new VersionNotRecognizedException(source.ToString(), $"Windows: WindowsVersions ({source.ToString()}) not supported!");
        }
    }
}
