using RepositoryWithUOW.Core.Entites;
using RepositoryWithUOW.Core.Interfaces;

namespace RepositoryWithUWO.EF.Repositories;

public class UnitOfWork(AppDbContext _appDbContext) : IUnitOfWork
{
    public IBaseRepository<Author> Authors { get; private set; } = new BaseRepository<Author>(_appDbContext);
    public IBookRepository Books { get; private set; } = new BookRepository(_appDbContext);

    public int Complete() => _appDbContext.SaveChanges();

    public async Task<int> CompleteAsync()
    {
        return await _appDbContext.SaveChangesAsync();
    }

    public void Dispose() => _appDbContext.Dispose();
}
