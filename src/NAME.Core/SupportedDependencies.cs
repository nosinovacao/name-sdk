using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Core
{
    /// <summary>
    /// Represents all the supported dependencies.
    /// </summary>
    public enum SupportedDependencies
    {
        /// <summary>
        /// MongoDb
        /// </summary>
        MongoDb = 10,
        
        /// <summary>
        /// Operating System
        /// </summary>
        OperatingSystem = 20,
        
        /// <summary>
        /// RabbitMq
        /// </summary>
        RabbitMq = 30,
        
        /// <summary>
        /// SQL Server
        /// </summary>
        SqlServer = 40,
        
        /// <summary>
        /// A service with NAME
        /// </summary>
        Service = 50
    }
}
