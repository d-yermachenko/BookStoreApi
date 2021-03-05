using AutoMapper;
using BookStoreApi.Code;
using BookStoreApi.Contracts;
using BookStoreApi.Data;
using BookStoreApi.Data.DTOs;
using BookStoreApi.Data.ModelBinders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace BookStoreApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase {

        private readonly ILogger<BooksController> _Logger;
        private readonly IBookStoreUnitOfWorkAsync _BookStore;
        private readonly IMapper _Mapper;
        private readonly IImageService _ImageService;

        public BooksController(ILogger<BooksController> logger,
            IBookStoreUnitOfWorkAsync bookStore,
            IMapper mapper,
            IImageService imageService) {
            _Logger = logger;
            _BookStore = bookStore;
            _Mapper = mapper;
            _ImageService = imageService;
        }

        private IActionResult InternalError(string message, Exception error = null) {
            if (error != null) {
                _Logger.LogError(error, message);
            }
            else
                _Logger.LogError(message);
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }

        private IActionResult BookBadRequest(string message, ModelStateDictionary modelState = null) {
            if (modelState != null)
                _Logger.LogWarning(message, Array.Empty<object>());
            else
                _Logger.LogWarning(message, modelState.Values.ToArray());

            if (modelState == null)
                return StatusCode(StatusCodes.Status400BadRequest, message);
            else
                return StatusCode(StatusCodes.Status400BadRequest, modelState);
        }

        private IActionResult BookNotFound(string message) {
            _Logger.LogWarning(message, Array.Empty<object>());
            return StatusCode(StatusCodes.Status404NotFound, message);
        }

        /// <summary>
        /// Creates book in books database
        /// </summary>
        /// <param name="book">New book object</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([ModelBinder(typeof(BookModelBinder))] BookUpsertDTO book) {
            _Logger.LogTrace("Attempt to create the book");
            try {
                if (book == null)
                    return BookBadRequest("Attempt to insert empty book");

                if (!ModelState.IsValid)
                    return BookBadRequest("Attempt to insert the invalid book", ModelState);
                Task<ImageData> imageUpload = null;
                CancellationTokenSource cts = null;
                if (book.ImageWasChanged) {
                    cts = new CancellationTokenSource();
                    imageUpload = _ImageService.SetImage(book.Image, cts.Token, String.Empty);
                    book.Image = String.Empty;
                }
                Book bookRecord = _Mapper.Map<Book>(book);
                var differences = DataTools.FindDifferences<BookAuthor, AuthorUpsertDTO>(
                    bookRecord.BookAuthors, book.Authors, (old, nw) => old.AuthorId == nw.Id);
                foreach (var inserted in differences.InsertedItems) {
                    Author author = await _BookStore.Authors.FindAsync(x => x.Id == inserted.Id);
                    if (author != null)
                        bookRecord.BookAuthors.Add(new BookAuthor() { AuthorId = inserted.Id, Author = author, Book = bookRecord });
                    else {
                        cts?.Cancel();
                        return StatusCode(StatusCodes.Status422UnprocessableEntity, "One of the book authors not exists in database, operation cancelled");
                    }
                }
                foreach (var removed in differences.RemovedItems)
                    bookRecord.BookAuthors.Remove(removed);
                if (imageUpload != null) {
                    ImageData imageData = await imageUpload;
                    bookRecord.Image = imageData.Name;
                    bookRecord.Thumbnail = imageData.Base64ThumbNail;
                }
                bool result = await _BookStore.Books.CreateAsync(bookRecord);
                result &= await _BookStore.SaveData();
                if (result) {
                    _Logger.LogInformation($"Book succesfully created, assigned id {bookRecord.Id}");
                    return StatusCode(StatusCodes.Status201Created, bookRecord);
                }
                else
                    return InternalError("Failed to create book");
            }
            catch (AggregateException e) {
                _Logger.LogError(e.Flatten(), e.Flatten().Message, e.Flatten().InnerExceptions?.Select(ie => $"{ie.Message}/n{new string('-', 20)}/n{ie.StackTrace}"));
                return InternalError("Failed to create the book", e);
            }
            catch (Exception e) {
                _Logger.LogError(e, e.Message);
                return InternalError("Failed to create bool", e);
            }
        }

        /// <summary>
        /// Gets list of books in the database
        /// </summary>
        /// <returns>All books</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks() {
            _Logger.LogTrace("Getting authors");
            try {
                var books = await _BookStore.Books.WhereAsync(order: ord => ord.OrderBy(x => x.Title)
                    , includes: new Expression<Func<Book, object>>[] { incl => incl.Authors });
                if (books.Count == 0)
                    return BookNotFound("Empty books list");
                var booksDTOs = _Mapper.Map<IEnumerable<BookDTO>>(books);
                _Logger.LogTrace("Succesully got the list of books");
                return Ok(booksDTOs);

            }
            catch (Exception e) {
                return InternalError("Failed to get list of books", e);
            }
        }

        /// <summary>
        /// Get book by its id
        /// </summary>
        /// <param name="id">Book Id</param>
        /// <returns>Book, if found</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id) {
            _Logger.LogTrace($"Getting author #{id}");
            try {
                var book = await _BookStore.Books.FindAsync(idPredicate: (x) => x.Id == id,
                    includes: new Expression<Func<Book, object>>[] { incl => incl.Authors });
                if (book == null)
                    return BookNotFound($"Book with id {id} not found in database");

                var booksDTOs = _Mapper.Map<BookDTO>(book);
                if (!String.IsNullOrWhiteSpace(book.Image)) {
                    var bookData = await _ImageService.GetImage(book.Image);
                    booksDTOs.Image = bookData.Base64Image;
                    booksDTOs.ImageMimeType = bookData.MediaType;
                }
                return Ok(booksDTOs);
            }
            catch (Exception e) {

                return InternalError("Failed to get list of books", e);
            }
        }

        /// <summary>
        /// Updating book in database
        /// </summary>
        /// <param name="bookId">Id of book to update</param>
        /// <param name="book">Updated book object</param>
        /// <returns>Updated book</returns>
        [HttpPut("{bookId}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int bookId, [ModelBinder(typeof(BookModelBinder))] BookUpsertDTO book) {
            _Logger.LogInformation("Attempting to update book");
            try {
                if (book == null)
                    return BookBadRequest("Book object must be not empty");
                if (bookId <= 0)
                    return BookBadRequest($"Book id you provided ({bookId}) is inferior of 0 and not exists in database");
                if (!ModelState.IsValid)
                    return BookBadRequest("Book data not passed the validation", ModelState);

                var srcBook = await _BookStore.Books.FindAsync(idPredicate: (x) => x.Id == bookId,
                    includes: new Expression<Func<Book, object>>[] { x => x.BookAuthors });
                if (srcBook == null)
                    return BookNotFound($"Book id you provided ({bookId}) not found and can not be updated");
                Task<ImageData> imageTask = null;
                CancellationTokenSource cancellationTokenSource = null;
                string originalImageName = srcBook.Image;
                if (book.ImageWasChanged) {
                    cancellationTokenSource = new CancellationTokenSource();
                    imageTask = _ImageService.SetImage(book.Image, cancellationTokenSource.Token, srcBook.Image);
                    book.Image = srcBook.Image;
                }
                else
                    book.Image = srcBook.Image;

                #region Updating authors list
                var differences = DataTools.FindDifferences<BookAuthor, AuthorUpsertDTO>(
                    srcBook.BookAuthors, book.Authors, (old, nw) => old.AuthorId == nw.Id);
                foreach (var inserted in differences.InsertedItems) {
                    Author author = await _BookStore.Authors.FindAsync(x => x.Id == inserted.Id);
                    if (author != null)
                        srcBook.BookAuthors.Add(new BookAuthor() { AuthorId = inserted.Id, Author = author, Book = srcBook, BookId = srcBook.Id });
                    else {
                        cancellationTokenSource?.Cancel();
                        return StatusCode(StatusCodes.Status422UnprocessableEntity, "One of the book authors not exists in database, operation cancelled");
                    }
                }
                foreach (var removed in differences.RemovedItems)
                    srcBook.BookAuthors.Remove(removed);
                srcBook = _Mapper.Map<BookUpsertDTO, Book>(book, srcBook);
                #endregion
                if (imageTask != null) {
                    var imageData = await imageTask;
                    srcBook.Image = imageData.Name;
                    srcBook.Thumbnail = imageData.Base64ThumbNail;
                }
                bool result = await _BookStore.Books.UpdateAsync(srcBook);
                result &= await _BookStore.SaveData();
                if (result) {
                    _Logger.LogTrace("Book id {bookId} was updated in the database");
                    return StatusCode(StatusCodes.Status200OK, srcBook);
                }
                else
                    return InternalError($"Failed to update book {bookId}");
            }
            catch (Exception e) {
                return InternalError("Failed to update the book", e);
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id) {
            _Logger.LogTrace($"Attempting to remove book # {id}");
            try {
                if (id <= 0)
                    return BadRequest($"Id you provided({id}) is loss of 0 and incorrect");
                var model = await _BookStore.Books.FindAsync(x => x.Id == id);
                if (model != null) {
                    if (!String.IsNullOrWhiteSpace(model.Image))
                        await _ImageService.RemoveImage(model.Image);
                }
                else
                    return NotFound($"Book with id {id} not found");

                bool success = await _BookStore.Books.DeleteAsync(id);
                success &= await _BookStore.SaveData();
                if (success) {
                    _Logger.LogTrace($"Successuly deleted book with id {id}.");
                    return StatusCode(StatusCodes.Status204NoContent);
                }
                else
                    return InternalError($"Filed to delete the book with id {id}.");

            }
            catch (KeyNotFoundException e) {
                return NotFound(e.Message);
            }
            catch (Exception e) {
                return InternalError($"Failed to remove book # {id}", e);
            }

        }
    }
}
