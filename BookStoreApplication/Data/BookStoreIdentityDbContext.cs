using BookStoreApi.Data.Authentification;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Data {
    public class BookStoreIdentityDbContext : IdentityDbContext<AppUser, AppRole, string> {

        private readonly IConfiguration _AppConfiguration;
        public BookStoreIdentityDbContext(IConfiguration configuration) {
            _AppConfiguration = configuration;
        }

        public BookStoreIdentityDbContext(DbContextOptions<BookStoreIdentityDbContext> options, IConfiguration configuration)
            : base(options) {
            _AppConfiguration = configuration;
        }
        protected override void OnModelCreating(ModelBuilder builder) {
            string dataSchema = _AppConfiguration.GetValue<String>("DbContext:AuthentificationSchema", "");
            if (!String.IsNullOrEmpty(dataSchema))
                builder.HasDefaultSchema(dataSchema);
            base.OnModelCreating(builder);
        }
    }
}
