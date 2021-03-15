using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Data.DTOs {

    /// <summary>
    /// DTO for user login
    /// </summary>
    public class UserLoginDTO {
        /// <summary>
        /// Login
        /// </summary>
        [Required]
        public string Login { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }

    /// <summary>
    /// Represents login data
    /// </summary>
    public class UserLoginData {
        /// <summary>
        /// Authentification answer message
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// Authentification token
        /// </summary>
        public string Token { get; set; }
    }

    /// <summary>
    /// Represents profile data
    /// </summary>
    public class UserProfileDTO {

    }

    /// <summary>
    /// Dro for user registration
    /// </summary>
    public class UserRegisterDTO {

        /// <summary>
        /// Login. Can be user mail or login
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "User login required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "User login must be between {2} and {1} characters length")]
        public string UserLogin { get; set; }

        /// <summary>
        /// Email. Principal email address
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "User email required")]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }


        /// <summary>
        /// User password
        /// </summary>
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Password confirmation
        /// </summary>
        [DataType(DataType.Password)]
        [Required]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string PasswordConfirmation { get; set; }

    }

    /// <summary>
    /// Change password DTO
    /// </summary>
    public class UserChangePasswordDTO {

        /// <summary>
        /// Current password
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Current password", Description = "Actual password")]
        public string CurrentPassword { get; set; }

        /// <summary>
        /// New password
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "New password", Description = "New password")]
        public string NewPassword { get; set; }

        /// <summary>
        /// New password confirmation
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "New password confirmation", Description = "New password confirmation")]
        [Compare("NewPassword", ErrorMessage ="Passwords must match")]
        public string NewPasswordConfirm { get; set; }
    }

    /// <summary>
    /// Reset password data object
    /// </summary>
    public class UserRequestResetPasswordDTO {

        /// <summary>
        /// User email to send password reset token. 
        /// </summary>
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

    }

    /// <summary>
    /// Uses to provide new data about password
    /// </summary>
    public class UserResponceResetPassword {
        /// <summary>
        /// Password reset token value
        /// </summary>
        public string ResetPasswordTokenValue { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [Required]
        public string UserEmail { get; set; }

        /// <summary>
        /// New password
        /// </summary>
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }


        /// <summary>
        /// New password confirmation
        /// </summary>
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords must match")]
        public string NewPasswordConfirm { get; set; }
    }

    public class PasswordActionAnswer {
        public string ConsernedUserMail { get; set; }

        public string ServerMessage { get; set; }
    }
}
