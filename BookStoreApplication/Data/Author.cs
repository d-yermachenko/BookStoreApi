using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace BookStoreApi.Data
{

    public partial class Author
    {
        public Author()
        {
            AuthorBooks = new List<BookAuthor>();
            Books = new List<Book>();
        }

        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Bio { get; set; }

        public virtual IList<Book> Books { get; set; }

        public virtual IList<BookAuthor> AuthorBooks { get; set; }

        public string OwnerId { get; set; }
    }
}
