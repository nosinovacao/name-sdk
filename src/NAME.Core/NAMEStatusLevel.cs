using NAME.Core;

namespace NAME
{
    /// <summary>
    /// Represents the status level of a NAME operation.
    /// Ordered by level of severity.
    /// </summary>
    public enum NAMEStatusLevel
    {
        /// <summary>
        /// No errors were found.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// The status of a dependency could not be resolved. E.g.: NAME not installed.
        /// </summary>
        Warn = 10,

        /// <summary>
        /// The status of a dependency was successfully resolved and is not valid.
        /// </summary>
        Error = 20
    }
}
