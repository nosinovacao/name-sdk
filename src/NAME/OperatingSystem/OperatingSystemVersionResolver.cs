using NAME.Core;
using NAME.Core.Exceptions;
using NAME.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NAME.OperatingSystem
{
    /// <summary>
    /// Provides a mechanism to get the version of the current Operating System.
    /// </summary>
    ///// <seealso cref="IVersionResolver{TVersion}" />
    public class OperatingSystemVersionResolver : IVersionResolver
    {
        private const string LINUX_OS_RELEASE_FILE = "/etc/os-release";
        private const string LINUX_OS_RELEASE_DISTRO_NAME_KEY = "ID";
        private const string LINUX_OS_RELEASE_DISTRO_VERSION_KEY = "VERSION_ID";
        
        /// <summary>
        /// Gets the versions.
        /// </summary>
        /// <returns>
        /// Returns an enumerable with one element only, with the Operating System version.
        /// </returns>
        /// <exception cref="NAMEException">Happens when the current Operating System is not supported.</exception>
        Task<IEnumerable<DependencyVersion>> IVersionResolver.GetVersions()
        {
            OperatingSystemDependencyVersion result;
#if NET462
            result = this.HandleWindowsOperatingSystem();
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                result = this.HandleLinuxOperatingSystem();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                result = this.HandleWindowsOperatingSystem();
            //TODO: Implement version fetching for OSX.
            else
                throw new NAMEException($"{SupportedDependencies.OperatingSystem.ToString()}: The current Operating System is not supported!", NAMEStatusLevel.Error);
#endif

            return Task.FromResult(new List<DependencyVersion> { result }.AsEnumerable());
        }
#if !NET462
        private OperatingSystemDependencyVersion HandleLinuxOperatingSystem()
        {
            if (!File.Exists(LINUX_OS_RELEASE_FILE))
                throw new NAMEException($"Linux: The file {LINUX_OS_RELEASE_FILE} does not exist or not enough permissions.", NAMEStatusLevel.Warn);

            var osReleaseLines = File.ReadAllLines(LINUX_OS_RELEASE_FILE);
            string distroId = null;
            string distroVersion = null;
            foreach (string line in osReleaseLines)
            {
                if (line.StartsWith(LINUX_OS_RELEASE_DISTRO_NAME_KEY + '=', StringComparison.Ordinal))
                    distroId = line.Split('=')[1].Trim('"');
                else if (line.StartsWith(LINUX_OS_RELEASE_DISTRO_VERSION_KEY + '=', StringComparison.Ordinal))
                    distroVersion = line.Split('=')[1].Trim('"');
            }

            if (distroId == null)
                throw new NAMEException($"Linux: Was unable to resolve the current distribution.", NAMEStatusLevel.Warn);

            if (distroVersion == null)
                throw new NAMEException($"Linux: Was unable to resolve the current version.", NAMEStatusLevel.Warn);
        
            if (!DependencyVersionParser.TryParse(distroVersion, false, out var parsedVersion))
                throw new NAMEException($"Linux: The extracted version ({distroVersion}) from the file {LINUX_OS_RELEASE_FILE} is not valid.", NAMEStatusLevel.Warn);

            return new OperatingSystemDependencyVersion(distroId, parsedVersion);
        }
#endif
        private OperatingSystemDependencyVersion HandleWindowsOperatingSystem()
        {
#if NET462
            return new OperatingSystemDependencyVersion("windows", (uint)Environment.OSVersion.Version.Major, (uint)Environment.OSVersion.Version.Minor, (uint)Environment.OSVersion.Version.Build);
#else
            string osDescription = RuntimeInformation.OSDescription;
            string versionPortion = osDescription.Split(new char[] { ' ' }, options: StringSplitOptions.RemoveEmptyEntries).Last();
            
            if (!DependencyVersionParser.TryParse(versionPortion, false, out var version))
                throw new NAMEException($"Windows: Was not able to extract the version from the OS description ({osDescription}).", NAMEStatusLevel.Warn);

            return new OperatingSystemDependencyVersion("windows", version);
#endif
        }
    }
}
