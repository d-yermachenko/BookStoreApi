using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.Data.DTOs {
    public class Author {


        [Display(Name = "Unique identity", Description = "Artificial and application specific number which identifiing")]
        public int Id { get; set; }

        [Display(Name = "First name", Description = "Author first name")]
        [MaxLength(50)]
        [Required]
        public string Firstname { get; set; }

        [Display(Name = "First name", Description = "Author first name")]
        [MaxLength(50)]
        [Required]
        public string Lastname { get; set; }

        [Display(Name = "First name", Description = "Author first name")]
        public string Bio { get; set; }

        public IList<Book> Books { get; set; }
    }

}
