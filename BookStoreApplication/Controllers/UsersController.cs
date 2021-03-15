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
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;

namespace BookStoreApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase {
        private readonly SignInManager<AppUser> _SignInManager;
        private readonly UserManager<AppUser> _UserManager;
        private readonly IConfiguration _Configuration;
        private readonly ILogger<UsersController> _Logger;
        private readonly IEmailSender _EmailSender;


        public UsersController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            ILogger<UsersController> logger,
            IEmailSender emailSender) {
            _UserManager = userManager;
            _SignInManager = signInManager;
            _Configuration = configuration;
            _Logger = logger;
            _EmailSender = emailSender;
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


        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Authorize]
        [Route("changepwd")]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDTO userChangePassword) {
            try {
                if (userChangePassword == null)
                    return StatusCode(StatusCodes.Status400BadRequest, new PasswordActionAnswer() {
                        ServerMessage = "Argument is null"
                    });
                if (!ModelState.IsValid)
                    return StatusCode(StatusCodes.Status400BadRequest, new PasswordActionAnswer() {
                        ServerMessage = ModelState.ToString()
                    });
                string userEmail = this.GetCurrentUserEmail();
                if (String.IsNullOrWhiteSpace(userEmail))
                    return StatusCode(StatusCodes.Status400BadRequest, new PasswordActionAnswer() {
                        ServerMessage = "Cant get current user data"
                    });
                var currentUser = await _UserManager.FindByEmailAsync(userEmail);
                if (currentUser == null)
                    return StatusCode(StatusCodes.Status404NotFound, new PasswordActionAnswer() {
                        ServerMessage = "User not found"
                    });
                var result = await _UserManager.ChangePasswordAsync(currentUser, userChangePassword.CurrentPassword, userChangePassword.NewPassword);
                if (result.Succeeded)
                    return StatusCode(StatusCodes.Status202Accepted, new PasswordActionAnswer() {
                        ServerMessage = "Password changed succesfully"
                    });
                else
                    return StatusCode(StatusCodes.Status417ExpectationFailed, new PasswordActionAnswer() {
                        ServerMessage = "Cant change password"
                    });

            }
            catch (Exception e) {
                _Logger.LogError("Change password operation failed because of exception", e);
                return StatusCode(StatusCodes.Status500InternalServerError, new PasswordActionAnswer() {
                    ServerMessage = "Something went wrong"
                });
            }

        }

        /// <summary>
        /// Resets forgotten password
        /// </summary>
        /// <param name="requestResetPassword"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost]
        [Route("resetpwd")]
        public async Task<IActionResult> ResetPassword([FromBody] UserRequestResetPasswordDTO requestResetPassword) {
            try {
                if (requestResetPassword == null)
                    return StatusCode(StatusCodes.Status400BadRequest, new PasswordActionAnswer() {
                        ServerMessage = "Object must be not null"
                    });
                if (!ModelState.IsValid)
                    return StatusCode(StatusCodes.Status400BadRequest, new PasswordActionAnswer() {
                        ServerMessage = ModelState.ToString()
                    });
                string userEmail = requestResetPassword.UserEmail;
                if (String.IsNullOrWhiteSpace(userEmail))
                    return StatusCode(StatusCodes.Status400BadRequest, new PasswordActionAnswer() {
                        ServerMessage = "Cant get current user data"
                    });
                var currentUser = await _UserManager.FindByEmailAsync(userEmail);
                if (currentUser == null)
                    return StatusCode(StatusCodes.Status404NotFound, new PasswordActionAnswer() {
                        ServerMessage = "User not found"
                    });
                var result = await _UserManager.GeneratePasswordResetTokenAsync(currentUser);
                result = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(result));
                if (_EmailSender != null) {
                    await _EmailSender.SendEmailAsync(currentUser.Email, "Password reset", $"Your password reset token: {result}");
                    return StatusCode(StatusCodes.Status200OK, new PasswordActionAnswer {
                        ConsernedUserMail = userEmail,
                        ServerMessage = "Email sent. Please insert code in password recovery form"
                    });
                }
                else {
                    return StatusCode(StatusCodes.Status500InternalServerError, new PasswordActionAnswer {
                        ConsernedUserMail = userEmail,
                        ServerMessage = "Cant initialize mail service"
                    });
                }
            }
            catch (Exception e) {
                _Logger.LogError("Change password operation failed because of exception", e);
                return StatusCode(StatusCodes.Status500InternalServerError, new PasswordActionAnswer() {
                    ServerMessage = "Something went wrong"
                });
            }
        }

        /// <summary>
        /// Sets new password, if valid token provided
        /// </summary>
        /// <param name="responce"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost]
        [Route("newpwdset")]
        public async Task<IActionResult> ResetPasswordSetNew([FromBody] UserResponceResetPassword responce) {
            try {
                if (responce == null)
                    return StatusCode(StatusCodes.Status400BadRequest, new PasswordActionAnswer() {
                        ServerMessage = "New password data required"
                    });
                string tokenValue;
                if (!String.IsNullOrWhiteSpace(responce?.ResetPasswordTokenValue))
                    tokenValue = responce.ResetPasswordTokenValue;
                else
                    tokenValue = String.Empty;
                if (!ModelState.IsValid)
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                if (String.IsNullOrWhiteSpace(tokenValue))
                    return StatusCode(StatusCodes.Status400BadRequest, new PasswordActionAnswer() {
                        ServerMessage = "Token value must be precised in query or in body"
                    });
                tokenValue = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(tokenValue));
                var user = await _UserManager.FindByEmailAsync(responce.UserEmail);
                if (user == null)
                    return StatusCode(StatusCodes.Status404NotFound, new PasswordActionAnswer() {
                        ServerMessage = "User not found",
                        ConsernedUserMail = responce.UserEmail
                    });
                var identityResult = await _UserManager.ResetPasswordAsync(user, tokenValue, responce.NewPassword);
                if (identityResult.Succeeded)
                    return StatusCode(StatusCodes.Status202Accepted, new PasswordActionAnswer() {
                    ServerMessage = "Password changed",
                        ConsernedUserMail = responce.UserEmail
                    }
                );
                else {
                    return StatusCode(StatusCodes.Status400BadRequest, new PasswordActionAnswer() {
                        ServerMessage = identityResult.Errors.ToString(),
                        ConsernedUserMail = responce.UserEmail
                    });
                }
            }
            catch (Exception e) {
                _Logger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new PasswordActionAnswer() {
                    ServerMessage = "Something went wrong",
                    ConsernedUserMail = responce.UserEmail
                });
            }
        }

        private async Task<string> GenerateBearerToken(AppUser user) {
            string result = string.Empty;
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration.GetValue<string>("JwtKey")));
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
