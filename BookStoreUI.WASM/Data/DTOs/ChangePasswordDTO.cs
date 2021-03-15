using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.WASM.Data.DTOs {
    public class ChangePasswordDTO {
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
        [Compare("NewPassword", ErrorMessage = "Passwords must match")]
        public string NewPasswordConfirm { get; set; }
    }
}
