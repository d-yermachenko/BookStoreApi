using BookStoreApi.Data.Authentification;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Data {
    public class BookStoreIdentityDbContext : IdentityDbContext<AppUser, AppRole, string> {
        public BookStoreIdentityDbContext() {
        }

        public BookStoreIdentityDbContext(DbContextOptions<BookStoreIdentityDbContext> options)
            : base(options) {
        } 
    }
}
