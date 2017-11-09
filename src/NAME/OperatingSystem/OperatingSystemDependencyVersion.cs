using NAME.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NAME.OperatingSystem
{
    /// <summary>
    /// Represents a combination of an Operating System and a version.
    /// </summary>
    public class OperatingSystemDependencyVersion : DependencyVersion
    {
        /// <summary>
        /// Gets the operating system.
        /// </summary>
        /// <value>
        /// The operating system.
        /// </value>
        public string OperatingSystem { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatingSystemDependencyVersion"/> class.
        /// </summary>
        /// <param name="operatingSystem">The operating system.</param>
        /// <param name="version">The version.</param>
        public OperatingSystemDependencyVersion(string operatingSystem, DependencyVersion version)
            : base(version.Major, version.Minor, version.Patch)
        {
            this.OperatingSystem = operatingSystem;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatingSystemDependencyVersion"/> class.
        /// </summary>
        /// <param name="operatingSystem">The operating system.</param>
        /// <param name="major">The major.</param>
        public OperatingSystemDependencyVersion(string operatingSystem, uint major)
            : base(major)
        {
            this.OperatingSystem = operatingSystem;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatingSystemDependencyVersion"/> class.
        /// </summary>
        /// <param name="operatingSystem">The operating system.</param>
        /// <param name="major">The major.</param>
        /// <param name="minor">The minor.</param>
        public OperatingSystemDependencyVersion(string operatingSystem, uint major, uint minor)
            : base(major, minor)
        {
            this.OperatingSystem = operatingSystem;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatingSystemDependencyVersion"/> class.
        /// </summary>
        /// <param name="operatingSystem">The operating system.</param>
        /// <param name="major">The major.</param>
        /// <param name="minor">The minor.</param>
        /// <param name="patch">The patch.</param>
        public OperatingSystemDependencyVersion(string operatingSystem, uint major, uint minor, uint patch)
            : base(major, minor, patch)
        {
            this.OperatingSystem = operatingSystem;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            var castedObj = obj as OperatingSystemDependencyVersion;
            if (ReferenceEquals(castedObj, null))
                return false;

            if (this.OperatingSystem.ToLower() != castedObj.OperatingSystem.ToLower())
                return false;

            return base.Equals(obj);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(OperatingSystemDependencyVersion left, OperatingSystemDependencyVersion right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(OperatingSystemDependencyVersion left, OperatingSystemDependencyVersion right)
        {
            return !(left == right);
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 17;
            hash = (hash * 31) + this.OperatingSystem.GetHashCode();
            hash = (hash * 31) + base.GetHashCode();
            return hash;
        }
    }
}
