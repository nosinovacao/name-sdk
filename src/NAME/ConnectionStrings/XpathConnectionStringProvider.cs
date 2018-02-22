using System;
using NAME.Core;
using System.Xml.XPath;
using System.IO;
using System.Diagnostics;

namespace NAME.ConnectionStrings
{
    /// <summary>
    /// Provides a mechanism to fetch a Connection String from an XMl file.
    /// </summary>
    /// <seealso cref="NAME.Core.IConnectionStringProvider" />
    public class XpathConnectionStringProvider : IConnectionStringProvider
    {
        private string xmlFilePath;
        private string xPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="XpathConnectionStringProvider" /> class.
        /// </summary>
        /// <param name="xmlFilePath">The XML file path.</param>
        /// <param name="xPath">The XPath expression.</param>
        public XpathConnectionStringProvider(string xmlFilePath, string xPath)
        {
            this.xmlFilePath = xmlFilePath;
            this.xPath = xPath;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="connectionString">A <see cref="string" /> containing the connection string.</param>
        /// <returns>
        /// Returns true if the connection string was fetched successfully.
        /// </returns>
        public bool TryGetConnectionString(out string connectionString)
        {
            connectionString = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(this.xmlFilePath, FileMode.Open, FileAccess.Read);
                XPathDocument document = new XPathDocument(fs);
                XPathNavigator navigator = document?.CreateNavigator();
                XPathExpression query = navigator?.Compile(this.xPath);

                object evaluatedObject = navigator?.Evaluate(query);
                connectionString = evaluatedObject as string;
                if (connectionString == null)
                {
                    var iterator = evaluatedObject as XPathNodeIterator;
                    if (iterator?.MoveNext() == true)
                        connectionString = iterator.Current?.Value;
                }

                return connectionString != null;
            }
            catch (Exception)
            {
                Trace.TraceInformation($"Could not fetch the connection string from the file {this.xmlFilePath} with the XPath {this.xPath}.");
                return false;
            }
            finally
            {
                fs?.Dispose();
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Locator: {SupportedConnectionStringLocators.XPath}, XPath: {this.xPath}, File: {this.xmlFilePath}";
        }
    }


}