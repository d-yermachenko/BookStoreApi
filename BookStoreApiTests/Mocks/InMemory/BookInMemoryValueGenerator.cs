using BookStoreApi.Data;

namespace BookStoreApiTests.Mocks.InMemory {
    internal class BookInMemoryValueGenerator : CustomInMemoryValueGenerator<Book, int> {
        public BookInMemoryValueGenerator() : base(x => x.Id, x => x + 1) {

        }

    }
}
