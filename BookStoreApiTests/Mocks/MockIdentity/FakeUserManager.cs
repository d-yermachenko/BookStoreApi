using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks.MockIdentity {
    public class FakeUserManager<TAppUser> : UserManager<TAppUser>
         where TAppUser : class {
        public FakeUserManager()
            : base(new Mock<IUserStore<TAppUser>>().Object,
              new Mock<IOptions<IdentityOptions>>().Object,
              new Mock<IPasswordHasher<TAppUser>>().Object,
              new IUserValidator<TAppUser>[0],
              new IPasswordValidator<TAppUser>[0],
              new Mock<ILookupNormalizer>().Object,
              new Mock<IdentityErrorDescriber>().Object,
              new Mock<IServiceProvider>().Object,
              new Mock<ILogger<UserManager<TAppUser>>>().Object) { }

        public override Task<IdentityResult> CreateAsync(TAppUser user, string password) {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> AddToRoleAsync(TAppUser user, string role) {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(TAppUser user) {
            return Task.FromResult(Guid.NewGuid().ToString());
        }

    }
}
