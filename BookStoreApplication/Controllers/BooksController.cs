﻿using AutoMapper;
using BookStoreApi.Contracts;
using BookStoreApi.Data;
using BookStoreApi.Data.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookStoreApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase {

        private readonly ILogger<BooksController> _Logger;
        private readonly IBookStoreUnitOfWorkAsync _BookStore;
        private readonly IMapper _Mapper;

        public BooksController(ILoggerFactory loggerFactory,
            IBookStoreUnitOfWorkAsync bookStore,
            IMapper mapper) {
            _Logger = loggerFactory.CreateLogger<BooksController>();
            _BookStore = bookStore;
            _Mapper = mapper;
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody]BookUpsertDTO book) {
            _Logger.LogTrace("Attempt to create the book");
            try {
                if (book == null)
                    return BookBadRequest("Attempt to insert empty book");

                if (!ModelState.IsValid) 
                    return BookBadRequest("Attempt to insert the invalid book", ModelState);

                Book bookRecord = _Mapper.Map<Book>(book);
                bool result = await _BookStore.Books.CreateAsync(bookRecord);
                result &= await _BookStore.SaveData();
                if (result) {
                    _Logger.LogInformation($"Book succesfully created, assigned id {bookRecord.Id}");
                    return StatusCode(StatusCodes.Status201Created, bookRecord);
                }
                else
                    return InternalError("Failed to create book");


            }
            catch (Exception e) {

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
                var books = await _BookStore.Books.WhereAsync(order: ord => ord.OrderBy(x => x.Title),
                    includes: new Expression<Func<Book, object>>[] {incl=>incl.Authors  });
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
                var book = await _BookStore.Books.FindAsync(filter: b=>b.Id == id,
                    includes: new Expression<Func<Book, object>>[] { incl => incl.Authors });
                if (book == null)
                    return BookNotFound($"Book with id {id} not found in database");

                var booksDTOs = _Mapper.Map<BookDTO>(book);
                return Ok(booksDTOs);
            }
            catch (Exception e) {

                return InternalError("Failed to get list of books", e);
            }
        }

        /// <summary>
        /// Updating book in database
        /// </summary>
        /// <param name="book">Updated book object</param>
        /// <returns>Updated book</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(BookUpsertDTO book) {
            _Logger.LogInformation("Attempting to update book");
            try {
                if (book == null)
                    return BookBadRequest("Book object must be not empty");
                int bookId = book.Id;
                if(bookId <= 0) 
                    return BookBadRequest($"Book id you provided ({bookId}) is inferior of 0 and not exists in database");
                if (!ModelState.IsValid)
                    return BookBadRequest("Book data not passed the validation", ModelState);
                var srcBook = await _BookStore.Books.FindAsync(x => x.Id == bookId);
                if (srcBook == null)
                    return BookNotFound($"Book id you provided ({bookId}) not found and can not be updated");

                srcBook = _Mapper.Map<BookUpsertDTO, Book>(book, srcBook);
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id) {
            _Logger.LogTrace($"Attempting to remove book # {id}");
            try {
                if (id <= 0)
                    return BadRequest($"Id you provided({id}) is loss of 0 and incorrect");
                var srcElement = await _BookStore.Books.FindAsync(x => x.Id == id);
                if (srcElement == null)
                    return BookNotFound($"Cant find the book with this id({id})");
                bool success = await _BookStore.Books.DeleteAsync(srcElement);
                success &= await _BookStore.SaveData();
                if (success) {
                    _Logger.LogTrace($"Successuly deleted book with id {id}.");
                    return StatusCode(StatusCodes.Status204NoContent);
                }
                else
                    return InternalError($"Filed to delete the book with id {id}.");

            }
            catch (Exception e) {
                return InternalError($"Failed to remove book # {id}", e);
            }

        }


        #region Disposing
        bool _Disposed;

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing) {
            if (_Disposed)
                return;
            if (disposing) {
                _BookStore.Dispose();
            }
            _Disposed = true;

        }

        #endregion
    }
}