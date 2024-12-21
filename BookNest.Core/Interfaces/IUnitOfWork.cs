using RepositoryWithUOW.Core.Entites;

namespace RepositoryWithUOW.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IBaseRepository<Author> Authors { get; }
        public IBaseRepository<Book> Books { get; }

        public Task<int> SaveAsync();

    }
}
