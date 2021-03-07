using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.WASM.Data.ViewModels.Authentification {
    public class RegistrationVM {
        [Required(AllowEmptyStrings = false, ErrorMessage = "User login required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "User login must be between {2} and {1} characters length")]
        public string UserLogin { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "User email required")]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }


        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string PasswordConfirmation { get; set; }
    }

}
