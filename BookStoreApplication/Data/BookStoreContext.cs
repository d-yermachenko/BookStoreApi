using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace BookStoreApi.Data {
    public partial class BookStoreContext : DbContext {

        private readonly IConfiguration _AppConfiguration;

        public BookStoreContext(IConfiguration configuration) : base() {
            _AppConfiguration = configuration;
        }

        public BookStoreContext(DbContextOptions<BookStoreContext> options, IConfiguration configuration) : base(options) {
            _AppConfiguration = configuration;
        }

        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Book> Books { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            string dataSchema = _AppConfiguration.GetValue<String>("DbContext:DataSchema", String.Empty);
            if (!String.IsNullOrEmpty(dataSchema))
                modelBuilder.HasDefaultSchema(dataSchema);
            modelBuilder.Entity<Author>(entity => {
                entity.Property(e => e.Firstname).HasMaxLength(50);

                entity.Property(e => e.Lastname).HasMaxLength(50);

                entity.HasMany<Book>(b => b.Books).WithMany(a => a.Authors)
                .UsingEntity<BookAuthor>(
                    ba => ba
                        .HasOne(a => a.Book)
                        .WithMany(b => b.BookAuthors)
                        .HasForeignKey(baid => baid.BookId)
                        .OnDelete(DeleteBehavior.Cascade),
                    ab => ab
                    .HasOne(b => b.Author)
                    .WithMany(a => a.AuthorBooks)
                    .HasForeignKey(abid => abid.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade)
                    );
            });

            modelBuilder.Entity<Book>(entity => {
                entity.Property(e => e.Image).HasMaxLength(int.MaxValue);

                entity.Property(e => e.Isbn)
                    .HasColumnName("ISBN")
                    .HasMaxLength(50);

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.Summary).HasMaxLength(150);

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasMany<Author>(b => b.Authors)
                .WithMany(b => b.Books)
                .UsingEntity<BookAuthor>(
                    ba => ba
                    .HasOne(a => a.Author)
                    .WithMany(ab => ab.AuthorBooks)
                    .HasForeignKey(abid => abid.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade),
                    ab => ab
                    .HasOne(b => b.Book)
                    .WithMany(ba => ba.BookAuthors)
                    .HasForeignKey(baid => baid.BookId)
                    .OnDelete(DeleteBehavior.Cascade)
                    );
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
