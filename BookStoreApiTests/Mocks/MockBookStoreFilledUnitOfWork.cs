using BookStoreApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Mocks {
    public class MockBookStoreFilledUnitOfWork :MockBookStoreUnitOfWork {
        public MockBookStoreFilledUnitOfWork() : base() {
            Author aStugatsky = new Author() {
                Id = 1,
                Firstname = "Arkady",
                Lastname = "Strugatski",
                Bio = "He was exelent writer"
            };

            Author bStugatsky = new Author() {
                Id = 2,
                Firstname = "Boris",
                Lastname = "Strugatski",
                Bio = "He was exelent writer and scientist"
            };

            Author iAzymov = new Author() {
                Id = 3,
                Firstname = "Isaak",
                Lastname = "Azimov",
                Bio = "He is excellent writer and scientis"
            };

            Book mondayStartsSaturday = new Book() {
                Id = 1,
                Title = "Monday starts saturday",
                Year = 1960,
                Isbn = "978-5-17-106224-8",
                Summary = "Exelent partody to scientists community",
                Authors = new Author[] { aStugatsky, bStugatsky }
            };

            Book hardToBeTheGod = new Book() {
                Id = 2,
                Title = "Hard to be the God",
                Year = 1960,
                Isbn = "978-5-17-106224-8",
                Summary = "Another view what happens to civilizations",
                Authors = new Author[] { aStugatsky, bStugatsky }
            };

            Book theGodsThemselves = new Book() {
                Id = 3,
                Title = "The Gods Themselves",
                Year = 1972,
                Isbn = "978-8-49-800851-7",
                Summary = "Another view what happens to civilizations",
                Authors = new Author[] { iAzymov }
            };
            aStugatsky.Books = new Book[] { mondayStartsSaturday, hardToBeTheGod };
            bStugatsky.Books = new Book[] { mondayStartsSaturday, hardToBeTheGod };
            iAzymov.Books = new Book[] { theGodsThemselves };

            Authors.CreateAsync(aStugatsky);
            Authors.CreateAsync(bStugatsky);
            Authors.CreateAsync(iAzymov);
            Books.CreateAsync(mondayStartsSaturday);
            Books.CreateAsync(hardToBeTheGod);
            Books.CreateAsync(theGodsThemselves);
        }
    }
}
