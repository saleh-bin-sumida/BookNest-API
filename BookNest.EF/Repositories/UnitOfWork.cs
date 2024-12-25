using BookNest.EF.Data;
using RepositoryWithUOW.Core.Entites;
using RepositoryWithUOW.Core.Interfaces;

namespace RepositoryWithUWO.EF.Repositories;

public class UnitOfWork(AppDbContext _appDbContext) : IUnitOfWork
{
    public IBaseRepository<Author> Authors { get; private set; } = new BaseRepository<Author>(_appDbContext);
    public IBaseRepository<Book> Books { get; private set; } = new BaseRepository<Book>(_appDbContext);

    public void Dispose()
    {
        _appDbContext.Dispose();
    }

    public async Task<int> SaveAsync()
    {
        return await _appDbContext.SaveChangesAsync();
    }
}
