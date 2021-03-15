using BookStoreApi.Code.DataContoroller.Entity;
using BookStoreApi.Contracts;
using BookStoreApi.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks.InMemory {
    public class MockBookStoreInMemoryUnitOfWork : IBookStoreUnitOfWorkAsync {

        private readonly BookStoreContext _BookStoreContext;
        private readonly IRepositoryAsync<Author, int> _AuthorsRepository;
        private readonly IRepositoryAsync<Book, int> _BooksRepository;

        public MockBookStoreInMemoryUnitOfWork(MockBookStoreInMemoryContext storeContext) {
            _BookStoreContext = storeContext;
            _AuthorsRepository = new MockRepositoryInMemoryAsync<Author, int>(_BookStoreContext.Authors,
                (aut)=> {
                    var books = new List<Book>();
                    books.AddRange(aut.Books);
                    aut.Books = books;
                    var authorBooks = new List<BookAuthor>();
                    authorBooks.AddRange(aut.AuthorBooks);

                    aut.AuthorBooks = authorBooks;
                    return aut;
                });
            _BooksRepository = new MockRepositoryInMemoryAsync<Book, int>(_BookStoreContext.Books,
                (book)=> {
                    var authors = new List<Author>();
                    authors.AddRange(book.Authors);
                    book.Authors = authors;
                    var bookAuthors = new List<BookAuthor>();
                    bookAuthors.AddRange(bookAuthors);
                    book.BookAuthors = bookAuthors;
                    return book;
                });
        }

        public IRepositoryAsync<Author, int> Authors {
            get => _AuthorsRepository;
        }
        
        public IRepositoryAsync<Book, int> Books {
            get => _BooksRepository;
        }


        public async Task<bool> SaveData() {
            try {
                return (await _BookStoreContext.SaveChangesAsync() > 0);
            }
            catch(NotSupportedException e) {
                switch (e.Message) {
                    case "Collection was of a fixed size.":
                        return true;
                    default:
                        throw;
                }
            }
            catch (Exception) {
                throw;
            }
            
        }

    }
}
