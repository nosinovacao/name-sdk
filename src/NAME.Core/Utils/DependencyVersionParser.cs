using NAME.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace NAME.Core.Utils
{
    /// <summary>
    /// Provides mechanisms to parse <see cref="DependencyVersion"/>s from strings.
    /// </summary>
    public static class DependencyVersionParser
    {
        /// <summary>
        /// The pattern to match a semver
        /// </summary>
        private const string Pattern = @"^((?<majorWildcard>\*)|(?<major>\d+)((?<minorWildcard>\.\*)|(\.(?<minor>\d+)((?<patchWildcard>\.\*)|(\.(?<patch>\d+)))?))?)$";

        /// <summary>
        /// The version matcher
        /// </summary>
        private static readonly Regex VersionMatcher = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Parses a <see cref="DependencyVersion"/> from the specified version string.
        /// </summary>
        /// <param name="version">The version string.</param>
        /// <param name="acceptWildcards">if set to <c>true</c> the parsing will take into account wildcard parts and possibly return a <see cref="WildcardDependencyVersion"/>.</param>
        /// <returns>Returns a new instance of <see cref="DependencyVersion"/>.</returns>
        /// <exception cref="VersionParsingException">Happens when the version parts could not be parsed.</exception>
        public static DependencyVersion Parse(string version, bool acceptWildcards)
        {
            if (!TryParse(version, acceptWildcards, out DependencyVersion result))
                throw new VersionParsingException(version, $"The version ({version}) parts could not be parsed.");

            return result;
        }

        /// <summary>
        /// Tries to parse a <see cref="DependencyVersion" /> from the specified version string.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="acceptWildcards">if set to <c>true</c> the parsing will take into account wildcard parts and possibly return a <see cref="WildcardDependencyVersion"/>.</param>
        /// <param name="outVersion">The parsed version.</param>
        /// <returns>
        /// Returns true if the parsing was successfuly. Otherwise, returns false.
        /// </returns>
        /// <exception cref="NAMEException">The JSON returned from the service is not a valid manifest.</exception>
        public static bool TryParse(string version, bool acceptWildcards, out DependencyVersion outVersion)
        {
            outVersion = null;
            if (version == null)
                return false;

            var parsedVersion = ParseVersion(version);
            if (parsedVersion == null)
            {
                outVersion = default(DependencyVersion);
                return false;
            }

            if (!parsedVersion.Item1.HasValue || !parsedVersion.Item2.HasValue || !parsedVersion.Item3.HasValue)
            {
                if (acceptWildcards == false)
                    return false;
                if (!parsedVersion.Item1.HasValue)
                    outVersion = new WildcardDependencyVersion();
                else if (!parsedVersion.Item2.HasValue)
                    outVersion = new WildcardDependencyVersion(parsedVersion.Item1.Value);
                else
                    outVersion = new WildcardDependencyVersion(parsedVersion.Item1.Value, parsedVersion.Item2.Value);

                return true;
            }


            outVersion = new DependencyVersion(parsedVersion.Item1.Value, parsedVersion.Item2.Value, parsedVersion.Item3.Value);

            return true;
        }

        /// <summary>
        /// Parses the version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>the parsed version as anonymous type</returns>
        private static Tuple<uint?, uint?, uint?> ParseVersion(string version)
        {
            var match = VersionMatcher.Match(version);

            if (!match.Success)
                return null;

            if (match.Groups["majorWildcard"].Success)
                return Tuple.Create<uint?, uint?, uint?>(null, null, null);

            uint major = ParseVersionValue(match.Groups[nameof(major)]);

            if (match.Groups["minorWildcard"].Success)
                return Tuple.Create<uint?, uint?, uint?>(major, null, null);

            uint minor = ParseVersionValue(match.Groups[nameof(minor)]);

            if (match.Groups["patchWildcard"].Success)
                return Tuple.Create<uint?, uint?, uint?>(major, minor, null);

            uint patch = ParseVersionValue(match.Groups[nameof(patch)]);
            // I dream the day multiple returns are a thing
            return Tuple.Create<uint?, uint?, uint?>(major, minor, patch);
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
