using NAME.Core.Exceptions;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NAME.Json;

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
        public uint Major { get; }

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        /// <value>
        /// The minor version.
        /// </value>
        public uint Minor { get; }

        /// <summary>
        /// Gets the patch version.
        /// </summary>
        /// <value>
        /// The patch version.
        /// </value>
        public uint Patch { get; }

        /// <summary>
        /// Gets or sets the manifest jsonnode for this dependency.
        /// </summary>
        /// <value>
        /// The manifest.
        /// </value>
        internal JsonNode ManifestNode { get; set; }

        /// <summary>
        /// The pattern to match a semver
        /// </summary>
        private const string Pattern = @"^(?<major>\d+)(\.(?<minor>\d+)(\.(?<patch>\d+))?)?$";

        /// <summary>
        /// The version matcher
        /// </summary>
        private static readonly Regex VersionMatcher = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.Singleline);

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
        /// Parses a <see cref="DependencyVersion"/> from the specified version string.
        /// </summary>
        /// <param name="version">The version string.</param>
        /// <returns>Returns a new instance of <see cref="DependencyVersion"/>.</returns>
        /// <exception cref="VersionParsingException">Happens when the version parts could not be parsed.</exception>
        public static DependencyVersion Parse(string version)
        {

            if (!TryParse(version, out DependencyVersion result))
                throw new VersionParsingException(version, $"The version ({version}) parts could not be parsed.");

            return result;
        }

        /// <summary>
        /// Tries to parse a <see cref="DependencyVersion" /> from the specified version string.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="outVersion">The parsed version.</param>
        /// <returns>
        /// Returns true if the parsing was successfuly. Otherwise, returns false.
        /// </returns>
        /// <exception cref="NAMEException">The JSON returned from the service is not a valid manifest.</exception>
        public static bool TryParse(string version, out DependencyVersion outVersion)
        {
            if (version == null)
                throw new NAMEException($"{SupportedDependencies.Service}: The JSON returned from the service is not a valid manifest.");

            var parsedVersion = ParseVersion(version);
            if (parsedVersion == null)
            {
                outVersion = default(DependencyVersion);
                return false;
            }

            outVersion = new DependencyVersion(parsedVersion.Item1, parsedVersion.Item2, parsedVersion.Item3);
            return true;
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

            return Comparison(this, (DependencyVersion)obj) == 0;
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
            if (ReferenceEquals(version1, version2))
                return 0;

            if (ReferenceEquals(version1, null) && ReferenceEquals(version2, null))
                return 0;
            if (ReferenceEquals(version1, null))
                return -1;
            if (ReferenceEquals(version2, null))
                return 1;

            var r = version1.Major.CompareTo(version2.Major);
            if (r != 0)
                return r;

            r = version1.Minor.CompareTo(version2.Minor);
            if (r != 0)
                return r;

            r = version1.Patch.CompareTo(version2.Patch);
            if (r != 0)
                return r;

            return 0;
        }


        /// <summary>
        /// Performs a comparision of this DependencyVersion with another object.
        /// </summary>
        /// <param name="obj">The object to compare this DependencyVersion to.</param>
        /// <returns>Returns 1 if the first version is bigger than the second version.
        /// Returns -1 if the first version is lesser than the second version.
        /// Return 0 if both versions are equal.</returns>
        public int CompareTo(object obj)
        {
            if (!(obj is DependencyVersion))
                return 1;
            return Comparison(this, (DependencyVersion)obj);
        }

        /// <summary>
        /// Performs a comparision of this DependencyVersion with another DependencyVersion object.
        /// </summary>
        /// <param name="other">The DependencyVersion to compare this DependencyVersion to.</param>
        /// <returns>Returns 1 if the first version is bigger than the second version.
        /// Returns -1 if the first version is lesser than the second version.
        /// Return 0 if both versions are equal.</returns>
        public int CompareTo(DependencyVersion other)
        {
            return Comparison(this, other);
        }


        /// <summary>
        /// Parses the version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>the parsed version as anonymous type</returns>
        private static Tuple<uint, uint, uint> ParseVersion(string version)
        {
            var match = VersionMatcher.Match(version);

            if (!match.Success)
                return null;

            uint major = ParseVersionValue(match.Groups[nameof(major)]);
            uint minor = ParseVersionValue(match.Groups[nameof(minor)]);
            uint patch = ParseVersionValue(match.Groups[nameof(patch)]);
            // I dream the day multiple returns are a thing
            return Tuple.Create(major, minor, patch);
        }

        /// <summary>
        /// Parses the version value.
        /// </summary>
        /// <param name="group">The regex group.</param>
        /// <returns>the parsed number</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ParseVersionValue(Group group)
        {
            return group.Success ? uint.Parse(@group.Value) : 0u;
        }
    }
}
