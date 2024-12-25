using BookNest.EF.Data;
using Microsoft.EntityFrameworkCore;
using RepositoryWithUOW.Core.Constants;
using RepositoryWithUOW.Core.Interfaces;
using System.Linq.Expressions;


namespace RepositoryWithUWO.EF.Repositories;

public class BaseRepository<T>(AppDbContext appDbContext) : IBaseRepository<T> where T : class
{
    private AppDbContext _appDbContext = appDbContext;

    public async Task<IEnumerable<T>> GetAllAsync(string[] includes = null)
    {
        IQueryable<T> values = _appDbContext.Set<T>();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                values = values.Include(include);
            }
        }

        return await values.ToListAsync();
    }

    public async Task<T> FindAsync(Expression<Func<T, bool>> predicate, string[] includes = null)
    {
        IQueryable<T> values = _appDbContext.Set<T>();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                values = values.Include(include);
            }
        }
        return await values.SingleOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate, string[] includes = null)
    {
        IQueryable<T> values = _appDbContext.Set<T>();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                values = values.Include(include);
            }
        }
        return await values.Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate, int? skip = null, int? take = null,
        string orderByDirection = OrderByStrings.Ascending, Expression<Func<T, object>> orderByFunc = null, string[] includes = null)
    {
        IQueryable<T> values = _appDbContext.Set<T>().Where(predicate);

        if (includes != null)
        {
            foreach (var include in includes)
            {
                values = values.Include(include);
            }
        }

        if (orderByFunc != null)
        {
            values = orderByDirection == OrderByStrings.Ascending ? values.OrderBy(orderByFunc) : values.OrderByDescending(orderByFunc);
        }

        if (skip.HasValue)
        {
            values = values.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            values = values.Take(take.Value);
        }

        return await values.ToListAsync();
    }

    // AnyAscync
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => await _appDbContext.Set<T>().AnyAsync(predicate);

    public async Task AddAsync(T item) => await _appDbContext.Set<T>().AddAsync(item);

    public async Task UpdateAsync(T item) => _appDbContext.Set<T>().Update(item);

    public async Task DeleteAsync(int id) => _appDbContext.Remove(await _appDbContext.FindAsync<T>(id));
}
