namespace NAME.Core
{
    /// <summary>
    /// Represents the supported connection string locators.
    /// </summary>
    public enum SupportedConnectionStringLocators
    {
#if NET45
        /// <summary>
        /// ConfigurationManager ConnectionStrings.
        /// </summary>
        ConnectionStrings = 10,

        /// <summary>
        /// ConfigurationManager AppSettings.
        /// </summary>
        AppSettings = 20,

        /// <summary>
        /// Visual Studio generated settings file.
        /// </summary>
        VSSettingsFile = 30,
#endif

        /// <summary>
        /// JsonPath.
        /// </summary>
        JSONPath = 40,

        /// <summary>
        /// XPath.
        /// </summary>
        XPath = 50

    }
}
