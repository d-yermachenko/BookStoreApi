using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Data.DTOs {

    public class UserLoginDTO {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class UserLoginData {
        public string Answer { get; set; }

        public string Token { get; set; }
    }
}
