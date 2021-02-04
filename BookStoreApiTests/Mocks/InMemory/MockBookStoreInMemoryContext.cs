using BookStoreApi.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks.InMemory {
    public class MockBookStoreInMemoryContext : BookStoreContext{

        public MockBookStoreInMemoryContext() : base() {

        }

        public MockBookStoreInMemoryContext(DbContextOptions<BookStoreContext> options) : base(options) {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Author>().Property(x => x.Id).ValueGeneratedOnAdd().HasValueGenerator<AuthorInMemoryValueGenerator>();
            modelBuilder.Entity<Book>().Property(b => b.Id).ValueGeneratedOnAdd().HasValueGenerator<BookInMemoryValueGenerator>();
        }
    }
}
