using NAME.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.ConnectionStrings
{
    /// <summary>
    /// Represents a connection string value reader from a environment variable
    /// </summary>
    public class EnvironmentVariableConnectionStringProvider : IConnectionStringProvider
    {
        private string environmentVariable;
       
        /// <summary>
        /// creates a new instance of <see cref="EnvironmentVariableConnectionStringProvider"/>
        /// </summary>
        /// <param name="environmentVariable">environment variable to searchfor</param>
        public EnvironmentVariableConnectionStringProvider(string environmentVariable)
        {
            this.environmentVariable = environmentVariable;
        }

        /// <summary>
        ///  gets the connection string   
        /// </summary>
        /// <param name="connectionString">connection string to return</param>
        /// <returns>
        /// Returns true if the connection string was fetched successfully.
        /// </returns>
        public virtual bool TryGetConnectionString(out string connectionString)
        {
            connectionString = Environment.GetEnvironmentVariable(this.environmentVariable);

            return !string.IsNullOrWhiteSpace(connectionString);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Locator: {SupportedConnectionStringLocators.EnvironmentVariable}, EnvironmentVariable: {this.environmentVariable}";
        }

    }
}
