using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Core
{
    /// <summary>
    /// Represents a contract to translate an object to a DependencyVersion.
    /// </summary>
    ///// <typeparam name="TVersion">The type of the version output.</typeparam>
    public interface IVersionTranslator
    {
        /// <summary>
        /// Translates the specified string to a TVersion.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>Returns a TVersion that identifies the specified source.</returns>
        DependencyVersion Translate(string source);

        /// <summary>
        /// Translates the specified TVersion of an object of type T.
        /// </summary>
        /// <param name="source">The source TVersion.</param>
        /// <returns>Returns an instance of type T.</returns>
        string Translate(DependencyVersion source);
    }
}
