using BookStoreApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks.InMemory {
    public class MockBookStoreInMemoryContext : BookStoreContext{

        public MockBookStoreInMemoryContext(IConfiguration configuration) : base(configuration) {
        }

        public MockBookStoreInMemoryContext(DbContextOptions<BookStoreContext> options, IConfiguration configuration) : base(options, configuration) {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Author>().Property(x => x.Id).ValueGeneratedOnAdd().HasValueGenerator<AuthorInMemoryValueGenerator>();
            modelBuilder.Entity<Book>().Property(b => b.Id).ValueGeneratedOnAdd().HasValueGenerator<BookInMemoryValueGenerator>();
        }
    }
}
