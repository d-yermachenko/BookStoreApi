﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Data.Authentification {
    public class AppUser : IdentityUser {

        public AppUser() : base() {

        }
    }
}
