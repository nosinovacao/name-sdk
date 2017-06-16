using NAME.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Tests.Dependencies
{
    public class StaticVersionResolverMock : IVersionResolver
    {
        private IEnumerable<DependencyVersion> versionsToReturn;
        public StaticVersionResolverMock(IEnumerable<DependencyVersion> versionsToReturn)
        {
            this.versionsToReturn = versionsToReturn;
        }

        public Task<IEnumerable<DependencyVersion>> GetVersions()
        {
            return Task.FromResult(versionsToReturn);
        }
    }
}
