using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.Data.ViewModels.Authentification {
    public class UserLoginDTO {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }

}
