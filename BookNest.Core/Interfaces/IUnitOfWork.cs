using RepositoryWithUOW.Core.Entites;

namespace RepositoryWithUOW.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IBaseRepository<Author> Authors { get; }
        public IBookRepository Books { get; }


        public int Complete();
        public Task<int> CompleteAsync();

    }
}
