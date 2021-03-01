using BookStoreApi.Data;

namespace BookStoreApiTests.Mocks.InMemory {
    internal class AuthorInMemoryValueGenerator : CustomInMemoryValueGenerator<Author, int> {
        public AuthorInMemoryValueGenerator() : base(x => x.Id, x =>x+1) {

        }

    }
}
