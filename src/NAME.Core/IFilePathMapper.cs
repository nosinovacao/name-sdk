namespace NAME.Core
{
    /// <summary>
    /// Provides a mechanism to map relative file paths to absolute paths.
    /// </summary>
    public interface IFilePathMapper
    {
        /// <summary>
        /// Maps the specified path to an absolute path.
        /// </summary>
        /// <param name="filePath">The relative file path.</param>
        /// <returns>Returns the absolute file path.</returns>
        string MapPath(string filePath);
    }
}