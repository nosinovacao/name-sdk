using NAME.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NAME.Core;

namespace NAME.SqlServer
{
    /// <summary>
    /// Provides a mechanism to translate a <see cref="SqlServerVersions"/> to a <see cref="DependencyVersion"/>.
    /// </summary>
    ///// <seealso cref="IVersionTranslator{TVersion}" >/
    public class SqlServerVersionTranslator : IVersionTranslator
    {

        /// <summary>
        /// Translates the specified string to a <see cref="DependencyVersion" />.
        /// </summary>
        /// <param name="source">The source <see cref="SqlServerVersions"/></param>
        /// <returns>
        /// Returns a <see cref="DependencyVersion" /> that identifies the specified <see cref="SqlServerVersions"/>.
        /// </returns>
        /// <exception cref="VersionNotRecognizedException">Happens when the specified <see cref="SqlServerVersions"/> is not supported.</exception>
        public DependencyVersion Translate(string source)
        {
            if (source == SqlServerVersions.SqlServer7.ToString())
                return new DependencyVersion(7, 00, 623);
            if (source == SqlServerVersions.SqlServer2000.ToString())
                return new DependencyVersion(8, 00, 384);
            if (source == SqlServerVersions.SqlServer2005.ToString())
                return new DependencyVersion(9, 0, 1399);
            if (source == SqlServerVersions.SqlServer2008.ToString())
                return new DependencyVersion(10, 0, 1600);
            if (source == SqlServerVersions.SqlServer2008R2.ToString())
                return new DependencyVersion(10, 50, 1600);
            if (source == SqlServerVersions.SqlServer2012.ToString())
                return new DependencyVersion(11, 0, 2100);
            if (source == SqlServerVersions.SqlServer2014.ToString())
                return new DependencyVersion(12, 0, 2000);
            if (source == SqlServerVersions.SqlServer2016.ToString())
                return new DependencyVersion(13, 0, 2186);

            throw new VersionNotRecognizedException(source.ToString(), $"{SupportedDependencies.SqlServer}: DependencyVersion ({source.ToString()}) not supported!");
        }

        /// <summary>
        /// Translates the specified <see cref="DependencyVersion" /> to a <see cref="string"/>.
        /// </summary>
        /// <param name="source">The source <see cref="DependencyVersion" />.</param>
        /// <returns>
        /// Returns the corresponding <see cref="SqlServerVersions"/>.
        /// </returns>
        /// <exception cref="VersionNotRecognizedException">Happens when the specified <see cref="DependencyVersion"/> is not supported.</exception>
        public string Translate(DependencyVersion source)
        {
            switch (source.Major)
            {
                case 7:
                    return SqlServerVersions.SqlServer7.ToString();
                case 8:
                    return SqlServerVersions.SqlServer2000.ToString();
                case 9:
                    return SqlServerVersions.SqlServer2005.ToString();
                case 10:
                    if (source.Minor == 0)
                        return SqlServerVersions.SqlServer2008.ToString();
                    else if (source.Minor == 50)
                        return SqlServerVersions.SqlServer2008R2.ToString();
                    break;
                case 11:
                    return SqlServerVersions.SqlServer2012.ToString();
                case 12:
                    return SqlServerVersions.SqlServer2014.ToString();
                case 13:
                    return SqlServerVersions.SqlServer2016.ToString();
            }
            throw new VersionNotRecognizedException(source.ToString(), $"{SupportedDependencies.SqlServer}: DependencyVersion ({source.ToString()}) not supported!");
        }
    }
}
