using NAME.Core.Exceptions;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NAME.Json;
using NAME.Core.Utils;

namespace NAME.Core
{
    /// <summary>
    /// Represents a version.
    /// </summary>
    public class DependencyVersion : IComparable<DependencyVersion>, IComparable
    {
        /// <summary>
        /// Gets the major version.
        /// </summary>
        /// <value>
        /// The major version.
        /// </value>
        public virtual uint Major { get; }

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        /// <value>
        /// The minor version.
        /// </value>
        public virtual uint Minor { get; }

        /// <summary>
        /// Gets the patch version.
        /// </summary>
        /// <value>
        /// The patch version.
        /// </value>
        public virtual uint Patch { get; }

        /// <summary>
        /// Gets or sets the manifest jsonnode for this dependency.
        /// </summary>
        /// <value>
        /// The manifest.
        /// </value>
        internal JsonNode ManifestNode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyVersion"/> class.
        /// </summary>
        /// <param name="major">The major.</param>
        /// <param name="minor">The minor.</param>
        /// <param name="patch">The patch.</param>
        public DependencyVersion(uint major, uint minor = 0, uint patch = 0)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
        }

        /// <summary>
        /// Parses a <see cref="DependencyVersion" /> from the specified version string.
        /// This method is deprecated. Please use <see cref="DependencyVersionParser.Parse"/> instead./>
        /// </summary>
        /// <param name="version">The version string.</param>
        /// <returns>
        /// Returns a new instance of <see cref="DependencyVersion" />.
        /// </returns>
        [Obsolete("This method is deprecated. Please use DependencyVersionParser.Parse instead.")]
        public static DependencyVersion Parse(string version)
        {
            return DependencyVersionParser.Parse(version, false);
        }

        /// <summary>
        /// Tries to parse a <see cref="DependencyVersion" /> from the specified version string.
        /// This method is deprecated. Please use <see cref="DependencyVersionParser.TryParse"/> instead./>
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="outVersion">The parsed version.</param>
        /// <returns>
        /// Returns true if the parsing was successfuly. Otherwise, returns false.
        /// </returns>
        [Obsolete("This method is deprecated. Please use DependencyVersionParser.TryParse instead.")]
        public static bool TryParse(string version, out DependencyVersion outVersion)
        {
            return DependencyVersionParser.TryParse(version, false, out outVersion);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Major}.{this.Minor}.{this.Patch}";
        }


        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(DependencyVersion version1, DependencyVersion version2)
        {

            return Comparison(version1, version2) < 0;

        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(DependencyVersion version1, DependencyVersion version2)
        {

            return Comparison(version1, version2) > 0;

        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(DependencyVersion version1, DependencyVersion version2)
        {

            return Comparison(version1, version2) == 0;

        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(DependencyVersion version1, DependencyVersion version2)
        {
            return Comparison(version1, version2) != 0;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <=(DependencyVersion version1, DependencyVersion version2)
        {
            return Comparison(version1, version2) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >=(DependencyVersion version1, DependencyVersion version2)
        {
            return Comparison(version1, version2) >= 0;
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
            if (obj == null)
                return false;
            if (!(obj is DependencyVersion))
                return false;

            return this.CompareTo((DependencyVersion)obj) == 0;
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
            hash = (hash * 23) + this.Major.GetHashCode();
            hash = (hash * 23) + this.Minor.GetHashCode();
            hash = (hash * 23) + this.Patch.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Performs a comparision between two DependencyVersion objects.
        /// </summary>
        /// <param name="version1">The first version.</param>
        /// <param name="version2">The second version.</param>
        /// <returns>Returns 1 if the first version is bigger than the second version.
        /// Returns -1 if the first version is lesser than the second version.
        /// Return 0 if both versions are equal.</returns>
        public static int Comparison(DependencyVersion version1, DependencyVersion version2)
        {
            if (ReferenceEquals(version1, null) && ReferenceEquals(version2, null))
                return 0;
            if (ReferenceEquals(version1, null))
                return -1;

            return version1.CompareTo(version2);
        }


        /// <summary>
        /// Performs a comparision of this DependencyVersion with another object.
        /// </summary>
        /// <param name="obj">The object to compare this DependencyVersion to.</param>
        /// <returns>Returns 1 if the first version is bigger than the second version.
        /// Returns -1 if the first version is lesser than the second version.
        /// Return 0 if both versions are equal.</returns>
        public virtual int CompareTo(object obj)
        {
            if (!(obj is DependencyVersion))
                return 1;
            return this.CompareTo((DependencyVersion)obj);
        }

        /// <summary>
        /// Performs a comparision of this DependencyVersion with another DependencyVersion object.
        /// </summary>
        /// <param name="other">The DependencyVersion to compare this DependencyVersion to.</param>
        /// <returns>Returns 1 if the first version is bigger than the second version.
        /// Returns -1 if the first version is lesser than the second version.
        /// Return 0 if both versions are equal.</returns>
        public virtual int CompareTo(DependencyVersion other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            if (ReferenceEquals(other, null))
                return 1;

            var r = this.Major.CompareTo(other.Major);
            if (r != 0)
                return r;

            r = this.Minor.CompareTo(other.Minor);
            if (r != 0)
                return r;

            r = this.Patch.CompareTo(other.Patch);
            if (r != 0)
                return r;

            return 0;
        }

    }
}
