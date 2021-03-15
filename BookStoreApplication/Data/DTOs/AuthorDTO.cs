using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Data.DTOs {
    public class AuthorDTO {

        /// <summary>
        /// Author id
        /// </summary>
        [Display(Name = "Unique identity", Description = "Artificial and application specific number which identifiing")]
        public int Id { get; set; }

        [Display(Name = "First name", Description = "Author first name")]
        [MaxLength(50)]
        public string Firstname { get; set; }

        [Display(Name = "First name", Description = "Author first name")]
        [MaxLength(50)]
        public string Lastname { get; set; }

        [Display(Name = "First name", Description = "Author first name")]
        public string Bio { get; set; }

        public ICollection<BookDTO> Books { get; set; }

        public ActionsAvalaibility ActionsAvalaibility { get; set; }
    }

    /// <summary>
    /// Manages author update or insert operations
    /// </summary>
    public class AuthorUpsertDTO {
        /// <summary>
        /// AuthorId
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Firts name of the author
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string Firstname { get; set; }

        /// <summary>
        /// Last name of the author
        /// </summary>
        [Display(Name = "First name", Description = "Author first name")]
        [MaxLength(50)]
        [Required]
        public string Lastname { get; set; }

        /// <summary>
        /// Biography of the author
        /// </summary>
        public string Bio { get; set; }

        /// <summary>
        /// List of the books of the author
        /// </summary>
        public IList<BookUpsertDTO> Books { get; set; }
    }
}
