using NAME.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace NAME.Core
{
    /// <summary>
    /// Represents a <see cref="DependencyVersion"/> that supports wildcards.
    /// A version with wildcards expects that only part of the version is used for comparisions.
    /// </summary>
    public class WildcardDependencyVersion : DependencyVersion, IComparable<WildcardDependencyVersion>
    {
        /// <summary>
        /// Gets a value indicating whether this versions minor part is a wildcard.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this version minor part is a wildcard; otherwise, <c>false</c>.
        /// </value>
        public bool IsMinorWildcard { get; }

        /// <summary>
        /// Gets a value indicating whether this versions major part is a wildcard.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this version major part is a wildcard; otherwise, <c>false</c>.
        /// </value>
        public bool IsMajorWildcard { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardDependencyVersion"/> class.
        /// Using this overload will set the <see cref="DependencyVersion.Major"/>, <see cref="DependencyVersion.Minor"/> and <see cref="DependencyVersion.Patch"/> as wildcards.
        /// </summary>
        public WildcardDependencyVersion()
            : this(uint.MaxValue)
        {
            this.IsMajorWildcard = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardDependencyVersion"/> class.
        /// Using this overload will set both the <see cref="DependencyVersion.Minor"/> and <see cref="DependencyVersion.Patch"/> as wildcards.
        /// </summary>
        /// <param name="major">The major part of the version.</param>
        public WildcardDependencyVersion(uint major)
            : this(major, uint.MaxValue)
        {
            this.IsMinorWildcard = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardDependencyVersion"/> class.
        /// Using this overload will set the <see cref="DependencyVersion.Minor"/> as a wildcard.
        /// </summary>
        /// <param name="major">The major.</param>
        /// <param name="minor">The minor.</param>
        public WildcardDependencyVersion(uint major, uint minor)
            : base(major, minor, uint.MaxValue)
        {
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.IsMajorWildcard)
                return "*";

            if (this.IsMinorWildcard)
                return $"{this.Major}.*";

            return $"{this.Major}.{this.Minor}.*";
        }

        /// <summary>
        /// Performs a comparision of this WildcardDependencyVersion with another DependencyVersion object.
        /// </summary>
        /// <param name="other">The DependencyVersion to compare this WildcardDependencyVersion to.</param>
        /// <returns>Returns 1 if the first version is bigger than the second version.
        /// Returns -1 if the first version is lesser than the second version.
        /// Return 0 if both versions are equal.</returns>
        public override int CompareTo(DependencyVersion other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            if (ReferenceEquals(other, null))
                return 1;

            if (other is WildcardDependencyVersion)
            {
                return this.CompareTo((WildcardDependencyVersion)other);
            }

            if (this.IsMajorWildcard)
                return 1;

            var r = this.Major.CompareTo(other.Major);
            if (r != 0)
                return r;

            if (this.IsMinorWildcard)
                return 1;

            r = this.Minor.CompareTo(other.Minor);
            if (r != 0)
                return r;

            return 1;
        }

        /// <summary>
        /// Performs a comparision of this WildcardDependencyVersion with another object.
        /// </summary>
        /// <param name="obj">The object to compare this WildcardDependencyVersion to.</param>
        /// <returns>Returns 1 if the first version is bigger than the second version.
        /// Returns -1 if the first version is lesser than the second version.
        /// Return 0 if both versions are equal.</returns>
        public override int CompareTo(object obj)
        {
            if (!(obj is DependencyVersion))
                return 1;
            return this.CompareTo((DependencyVersion)obj);
        }

        /// <summary>
        /// Performs a comparision of this WildcardDependencyVersion with another WildcardDependencyVersion.
        /// </summary>
        /// <param name="other">The WildcardDependencyVersion to compare this WildcardDependencyVersion to.</param>
        /// <returns>Returns 1 if the first version is bigger than the second version.
        /// Returns -1 if the first version is lesser than the second version.
        /// Return 0 if both versions are equal.</returns>
        public int CompareTo(WildcardDependencyVersion other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            if (ReferenceEquals(other, null))
                return 1;

            // Major part
            if (this.IsMajorWildcard && other.IsMajorWildcard)
                return 0;
            if (this.IsMajorWildcard && !other.IsMajorWildcard)
                return 1;
            if (!this.IsMajorWildcard && this.IsMajorWildcard)
                return -1;

            var r = this.Major.CompareTo(other.Major);
            if (r != 0)
                return r;

            // Minor part
            if (this.IsMinorWildcard && other.IsMinorWildcard)
                return 0;
            if (this.IsMinorWildcard && !other.IsMinorWildcard)
                return 1;
            if (!this.IsMinorWildcard && other.IsMinorWildcard)
                return -1;


            r = this.Minor.CompareTo(other.Minor);
            if (r != 0)
                return r;

            return 0;
        }
    }
}
