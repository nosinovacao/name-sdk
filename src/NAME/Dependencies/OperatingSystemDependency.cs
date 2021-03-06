using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NAME.Core;
using NAME.Json;
using NAME.OperatingSystem;
using NAME.Core.Exceptions;

namespace NAME.Dependencies
{
    internal class OperatingSystemDependency : VersionedDependency
    {
        public OperatingSystemDependency()
            : base(new OperatingSystemVersionResolver())
        {
        }

        public override async Task<DependencyCheckStatus> GetStatus()
        {
            try
            {
                IEnumerable<DependencyVersion> actualVersions = await this.VersionResolver.GetVersions().ConfigureAwait(false);
                if (!actualVersions.Any())
                    return new DependencyCheckStatus(NAMEStatusLevel.Warn, message: "Could not fetch the actual versions.");

                foreach (var version in actualVersions)
                {
                    var osVersion = version as OperatingSystemDependencyVersion;

                    if (osVersion == null || !osVersion.OperatingSystem.Equals(this.OperatingSystemName, StringComparison.OrdinalIgnoreCase))
                    {
                        return new DependencyCheckStatus(NAMEStatusLevel.Error, message: $"Unsupported Operating system { osVersion?.OperatingSystem }.)");
                    }

                    if (version < this.MinimumVersion || (this.MaximumVersion != null && version > this.MaximumVersion))
                    {
                        return new DependencyCheckStatus(NAMEStatusLevel.Error, version: version, message: $"Unsupported version.)");
                    }
                }

                return new DependencyCheckStatus(NAMEStatusLevel.Ok, actualVersions.FirstOrDefault());
            }
            catch (NAMEException ex)
            {
                return new DependencyCheckStatus(ex.StatusLevel, message: ex.Message, innerException: ex.InnerException);
            }
            catch (Exception ex)
            {
                return new DependencyCheckStatus(NAMEStatusLevel.Error, message: ex.Message, innerException: ex.InnerException);
            }
        }

        public string OperatingSystemName { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Name) ? this.OperatingSystemName : this.Name;
        }
    }
}
