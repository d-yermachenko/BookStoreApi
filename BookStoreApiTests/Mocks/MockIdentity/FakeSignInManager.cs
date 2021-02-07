using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStoreApiTests.Mocks.MockIdentity {
    public class FakeSignInManager<TAppUser> : SignInManager<TAppUser>
        where TAppUser : class
        {
        public FakeSignInManager() 
                : base(new FakeUserManager<TAppUser>(),
                     new Mock<IHttpContextAccessor>().Object,
                     new Mock<IUserClaimsPrincipalFactory<TAppUser>>().Object,
                     new Mock<IOptions<IdentityOptions>>().Object,
                     new Mock<ILogger<SignInManager<TAppUser>>>().Object,
                     new Mock<IAuthenticationSchemeProvider>().Object,
                     new Mock<IUserConfirmation<TAppUser>>().Object) { }
    }
}
