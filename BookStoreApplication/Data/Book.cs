using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace BookStoreApi.Data
{
    public partial class Book
    {
        public Book() {
            BookAuthors = new List<BookAuthor>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public string Isbn { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }

        public string Thumbnail { get; set; }

        public decimal? Price { get; set; }

        public virtual ICollection<Author> Authors { get; set; }

        public IList<BookAuthor> BookAuthors{ get; set; }
    }
}
