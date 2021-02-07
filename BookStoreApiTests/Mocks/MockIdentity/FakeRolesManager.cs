using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks.MockIdentity {
    public class FakeRolesManager<TRole> : RoleManager<TRole>
        where TRole : class {
        public FakeRolesManager() : base(
            new Mock<IRoleStore<TRole>>().Object,
            new Mock<IEnumerable<IRoleValidator<TRole>>>().Object,
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<ILogger<FakeRolesManager<TRole>>>().Object
            ) { }

    }
}
