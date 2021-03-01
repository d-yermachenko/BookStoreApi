using BookStoreApi.Data.Authentification;
using BookStoreApi.Data.DTOs;
using BookStoreApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase {
        private readonly SignInManager<AppUser> _SignInManager;
        private readonly UserManager<AppUser> _UserManager;
        private readonly IConfiguration _Configuration;
        private readonly ILogger<UsersController> _Logger;


        public UsersController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            ILogger<UsersController> logger) {
            _UserManager = userManager;
            _SignInManager = signInManager;
            _Configuration = configuration;
            _Logger = logger;
        }

        /// <summary>
        /// User login endpoint
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLogin) {
            try {
                if (!ModelState.IsValid) {
                    _Logger.LogTrace(ModelState.ToString());
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);

                }
                var user = await _UserManager.FindByNameAsync(userLogin.Login);
                if (user == null)
                    user = await _UserManager.FindByEmailAsync(userLogin.Login);
                if (user == null) {
                    _Logger.LogInformation($"User {userLogin.Login} not found");
                    return StatusCode(StatusCodes.Status404NotFound, "User with provided credentials was not found");
                }

                var signInResult = await _SignInManager.PasswordSignInAsync(user, userLogin.Password, false, false);
                if (signInResult.Succeeded) {
                    _Logger.LogInformation($"User {user.UserName} signed in");
                    string token = await GenerateBearerToken(user);
                    UserLoginData userLoginData = new UserLoginData() {
                        Answer = $"User {user.UserName} signed in",
                        Token = token
                    };
                    return StatusCode(StatusCodes.Status202Accepted, userLoginData);
                }
                else {
                    _Logger.LogInformation($"User {user.UserName} was not signed in");
                    return StatusCode(StatusCodes.Status401Unauthorized, "Incorrect password");
                }
            }
            catch (Exception e) {
                _Logger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }



        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userData) {
            try {
                if (userData == null)
                    return StatusCode(StatusCodes.Status417ExpectationFailed, "user data must be not empty");
                if (!ModelState.IsValid)
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                string location = this.GetControllerActionNames();
                AppUser appUser = new AppUser() {
                    UserName = userData.UserLogin,
                    Email = userData.UserEmail,
                };
                var identityResult = await _UserManager.CreateAsync(appUser, userData.Password);
                if (!identityResult.Succeeded) {
                    _Logger.LogWarning("Failed to register user", identityResult.Errors.Select(e => $"{location}: {e.Code} - {e.Description}").ToArray());
                    return this.InternalError(_Logger, "Unable to register the user");
                }
                identityResult = await _UserManager.AddToRoleAsync(appUser, "Customer");
                if (!identityResult.Succeeded) {
                    _Logger.LogWarning("Failed to add user to default role", identityResult.Errors.Select(e => $"{location}: {e.Code} - {e.Description}").ToArray());
                    return this.InternalError(_Logger, "Unable to assign the user to customers");
                }
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception e) {
                _Logger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error produced, information transmitted to maintenance team");
            }


        }
        private async Task<string> GenerateBearerToken(AppUser user) {
            string result = string.Empty;
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration.GetValue<string>("Jwt:Key")));
            if (!TimeSpan.TryParse(_Configuration.GetValue<string>("Jvt:Validity"), out TimeSpan validity))
                validity = TimeSpan.FromDays(1);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha384);
            string issuer = _Configuration.GetValue<string>("Jwt:Issuer");
            var claims = new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            claims.AddRange((await _UserManager.GetRolesAsync(user)).Select(x => new Claim(ClaimTypes.Role, x)));
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims.AsEnumerable(),
                notBefore: DateTime.Now,
                expires: DateTime.Now.Add(validity),
                signingCredentials: credentials);
            result = new JwtSecurityTokenHandler().WriteToken(token);
            return result;
        }
    }
}
