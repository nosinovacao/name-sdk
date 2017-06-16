using NAME.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Tests
{
    public class DummyNotFoundConnectionStringProvider : IConnectionStringProvider
    {
        public bool TryGetConnectionString(out string connectionString)
        {
            connectionString = null;
            return false;
        }
    }
}
