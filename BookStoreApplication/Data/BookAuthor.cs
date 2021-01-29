using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Data {
    public class BookAuthor {
        public Author Author { get; set; }

        public int AuthorId { get; set; }


        public Book Book { get; set; }

        public int BookId { get; set; }


    }
}
