using AutoMapper;
using BookStoreApi.Code;
using BookStoreApi.Contracts;
using BookStoreApi.Data;
using BookStoreApi.Data.Authentification;
using BookStoreApi.Data.DTOs;
using BookStoreApi.Data.ModelBinders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStoreApi.Controllers {
    /// <summary>
    /// Endpoint used to interact with authors in the Book store database.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase {
        private readonly ILogger<AuthorsController> _Logger;
        private readonly IBookStoreUnitOfWorkAsync _BookStore;
        private readonly IMapper _Mapper;
        private readonly SignInManager<AppUser> _SignInManager;

        public AuthorsController(ILogger<AuthorsController> logger,
            IBookStoreUnitOfWorkAsync bookStore,
            IMapper mapper,
            SignInManager<AppUser> signInManager) {
            _Logger = logger;
            _BookStore = bookStore;
            _Mapper = mapper;
            _SignInManager = signInManager;
        }

        /// <summary>
        /// Gets all authors from database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors() {
            try {
                _Logger.LogTrace("Getting authors");
                var user = await _SignInManager.UserManager.FindByEmailAsync(this.GetCurrentUserEmail());
                bool isUserAdmin = !(user == null) && await _SignInManager.UserManager.IsInRoleAsync(user, AppDataSeeder.Administrator);
                var authors = await _BookStore.Authors.WhereAsync(
                    order: x => x.OrderBy(au => au.Lastname).ThenBy(au => au.Firstname),
                    includes: null);
                IList<AuthorDTO> authorDTOs = new List<AuthorDTO>();
                foreach (Author author in authors) {
                    AuthorDTO authorDTO = _Mapper.Map<AuthorDTO>(author);
                    authorDTO.ActionsAvalaibility = this.GetActionsAvalaibility(author.OwnerId, user?.Id, isUserAdmin);
                    authorDTOs.Add(authorDTO);
                }
                _Logger.LogTrace("Returning authors");
                if (authorDTOs.Count == 0)
                    return NotFound();
                return Ok(authorDTOs); ;
            }
            catch (Exception e) {
                return InternalError(e, "Getting authors provoked internal server error");
            }
        }

        /// <summary>
        /// Return the author by his Id
        /// </summary>
        /// <param name="id">Id of the author</param>
        /// <returns>Author data, if found</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id) {
            try {
                _Logger.LogTrace($"Attempting to get author with id={id}");
                
                var author = (await _BookStore.Authors.WhereAsync(filter: x => x.Id == id ,
                    includes: new Expression<Func<Author, object>>[] { x => x.Books })).FirstOrDefault();
                if (author == null) {
                    _Logger.LogWarning($"Failed to get author id={id}");
                    return NotFound();
                }
                var user = await _SignInManager.UserManager.FindByEmailAsync(this.GetCurrentUserEmail());
                bool isUserAdmin = !(user == null) && await _SignInManager.UserManager.IsInRoleAsync(user, AppDataSeeder.Administrator);
                AuthorDTO authorDTOs = _Mapper.Map<AuthorDTO>(author);
                authorDTOs.ActionsAvalaibility = this.GetActionsAvalaibility(author.OwnerId, user?.Id, isUserAdmin);
                _Logger.LogTrace($"Returning author with id {id}");
                return Ok(authorDTOs);
            }
            catch (Exception e) {
                return InternalError(e, $"Getting author #{id} provoked internal server error");
            }
        }

        private ObjectResult InternalError(Exception e, string message) {
            _Logger.LogError(e, e.Message, Array.Empty<object>());
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }

        /// <summary>
        /// Create author
        /// </summary>
        /// <param name="author">Author data</param>
        /// <returns>New author if succeed; otherwise - message what went wrong</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([ModelBinder(typeof(AuthorModelBinder))] AuthorUpsertDTO author) {
            _Logger.LogInformation($"Author submition");
            try {
                if (author == null) {
                    _Logger.LogWarning($"Empty request was submited");
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
                if (!ModelState.IsValid) {
                    _Logger.LogWarning(
                        $"Attempting to submit invalid data: {ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).Aggregate((fm, lm) => fm + '\n' + lm)}");
                    return BadRequest(ModelState);
                }
                var user = await _SignInManager.UserManager.FindByEmailAsync(this.GetCurrentUserEmail());
                var similarAuthors = await _BookStore.Authors.WhereAsync(filter: x =>
                    x.Firstname.ToLower().Equals(author.Firstname.ToLower()) &&
                    x.Lastname.ToLower().Equals(author.Firstname.ToLower())
                    && x.OwnerId == user.Id
                    );
                if (similarAuthors.Count > 0)
                    _Logger.LogWarning($"Possible duplicate for author {author.Firstname} {author.Lastname.ToUpper()}");
                Author writer = _Mapper.Map<Author>(author);
                writer.OwnerId = user.Id;
                var differences = DataTools.FindDifferences<Book, BookUpsertDTO>(
                    writer.Books, author.Books, (o, n) => o.Id == n.Id);
                if (differences.InsertedItems?.Count > 0) {
                    foreach (var newAuthorBook in differences.InsertedItems) {
                        var authorBook = await _BookStore.Books.FindAsync(b => b.Id == newAuthorBook.Id);
                        if (authorBook != null)
                            writer.AuthorBooks.Add(new BookAuthor() { Author = writer, BookId = authorBook.Id, Book = authorBook });
                        else {
                            string message = $"Failed to find book with id {newAuthorBook.Id}";
                            _Logger.LogWarning(message);
                            return StatusCode(StatusCodes.Status422UnprocessableEntity, message);
                        }
                    }
                }
                var isSucceed = await _BookStore.Authors.CreateAsync(writer);
                isSucceed &= await _BookStore.SaveData();
                if (!isSucceed) {
                    _Logger.LogError("No records was inserted to database when creating the author");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to save author");
                }
                else {
                    _Logger.LogInformation("Author creates successfully");
                    return Created("Create", _Mapper.Map<AuthorUpsertDTO>(writer));
                }

            }
            catch (Exception e) {
                return InternalError(e, $"Unable to create author {author.Firstname} {author.Lastname}");
            }
        }

        /// <summary>
        /// Updates author data in the database
        /// </summary>
        /// <param name="authorId">Author id</param>
        /// <param name="author">Author </param>
        /// <returns></returns>
        [HttpPut("{authorId}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int authorId, [ModelBinder(typeof(AuthorModelBinder))] AuthorUpsertDTO author) {
            try {
                _Logger.LogTrace($"Author {authorId} attempt");
                if (author == null) {
                    _Logger.LogTrace($"Author {authorId} attempt");
                    return StatusCode(StatusCodes.Status400BadRequest, "Empty objecy is not allowed");
                }
                if (!ModelState.IsValid)
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                var authorToUpdate = await _BookStore.Authors.FindAsync(
                    idPredicate: (x) => x.Id == authorId,
                    includes: new Expression<Func<Author, object>>[] { x => x.Books });
                if (authorToUpdate == null)
                    return NotFound($"Unable to find author to update");
                var user = await _SignInManager.UserManager.FindByEmailAsync(this.GetCurrentUserEmail());
                var isUserAdmin = await _SignInManager.UserManager.IsInRoleAsync(user, AppDataSeeder.Administrator);
                if (!(authorToUpdate.OwnerId == user.Id || isUserAdmin)) {
                    _Logger.LogWarning($"{user.Email} tried edit others author");
                    return StatusCode(StatusCodes.Status403Forbidden, "Cant edit other's record");
                }

                var differences = DataTools.FindDifferences<Book, BookUpsertDTO>(
                   authorToUpdate.Books, author.Books, (o, n) => o.Id == n.Id);
                if (differences.InsertedItems?.Count > 0) {
                    foreach (var newAuthorBook in differences.InsertedItems) {
                        var authorBook = await _BookStore.Books.FindAsync(b => b.Id == newAuthorBook.Id);
                        if (authorBook != null)
                            authorToUpdate.AuthorBooks.Add(new BookAuthor() { Author = authorToUpdate, AuthorId = authorToUpdate.Id, BookId = authorBook.Id, Book = authorBook });
                        else {
                            string message = $"Failed to find book with id {newAuthorBook.Id}";
                            _Logger.LogWarning(message);
                            return StatusCode(StatusCodes.Status422UnprocessableEntity, message);
                        }
                    }
                    foreach (var removedAuthorBook in differences.RemovedItems)
                        authorToUpdate.Books.Remove(removedAuthorBook);
                }
                authorToUpdate = _Mapper.Map<AuthorUpsertDTO, Author>(author, authorToUpdate);
                bool succeed = await _BookStore.Authors.UpdateAsync(authorToUpdate);
                succeed &= await _BookStore.SaveData();
                if (succeed)
                    return StatusCode(StatusCodes.Status200OK, _Mapper.Map<AuthorUpsertDTO>(authorToUpdate));
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error while updating the author");

            }
            catch (Exception e) {
                return InternalError(e, "Unexpected error while updating the author");
            }
        }

        /// <summary>
        /// Removes author by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id) {
            try {
                var author = await _BookStore.Authors.FindAsync(x => x.Id == id);
                if(author == null)
                    return StatusCode(StatusCodes.Status404NotFound, "Author not found");
                var user = await _SignInManager.UserManager.FindByEmailAsync(this.GetCurrentUserEmail());
                var isUserAdministrator = await _SignInManager.UserManager.IsInRoleAsync(user, AppDataSeeder.Administrator);
                if (!(isUserAdministrator || author.OwnerId == user.Id )) {
                    _Logger.LogWarning($"{user.Email} tried edit others author");
                    return StatusCode(StatusCodes.Status403Forbidden, "Cant delete other's record");
                }
                bool succeed = await _BookStore.Authors.DeleteAsync(id);
                succeed &= await _BookStore.SaveData();
                if (succeed)
                    return StatusCode(StatusCodes.Status204NoContent);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unable to remove ");
            }
            catch (KeyNotFoundException e) {
                return NotFound(e.Message);
            }
            catch (Exception e) {
                return InternalError(e, "Cant delete author");
            }
        }
    }
}
