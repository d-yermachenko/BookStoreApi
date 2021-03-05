using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Data.DTOs {
    public class BookDTO {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public string Isbn { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }

        public string ImageMimeType { get; set; }

        public string Thumbnail { get; set; }

        public decimal? Price { get; set; }

        public virtual ICollection<Author> Authors { get; set; }
    }

    public class BookUpsertDTO {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public int? Year { get; set; }

        [Required]
        [RegularExpression(@"^(?:ISBN(?:-13)?:?\ )?(?=[0-9]{13}$|(?=(?:[0-9]+[-\ ]){4})[-\ 0-9]{17}$)97[89][-\ ]?[0-9]{1,5}[-\ ]?[0-9]+[-\ ]?[0-9]+[-\ ]?[0-9]$", ErrorMessage = "ISBN not formatted as ISBN")]
        public string Isbn { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }

        public string ImageMimeType { get; set; }
        public bool ImageWasChanged { get; set; }
        public decimal? Price { get; set; }

        public virtual IList<AuthorUpsertDTO> Authors { get; set; }
    }
}
