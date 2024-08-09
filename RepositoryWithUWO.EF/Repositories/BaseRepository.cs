using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUOW.Core.Constants;


using System.Linq.Expressions;
using System.Linq;
using System.Diagnostics.Metrics;


namespace RepositoryWithUWO.EF.Repositories;

public class BaseRepository<T>(AppDbContext appDbContext) : IBaseRepository<T> where T : class
{
    private AppDbContext _appDbContext  = appDbContext;



    public async Task<IEnumerable<T>> IndexAsync(string[] includes = null)
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

    public async Task< T> Find(Expression<Func<T, bool>> predicate, string[] includes = null)
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
        IQueryable<T> values =  _appDbContext.Set<T>();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                values = values.Include(include);
            }
        }
        return await values.Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate,  int? skip = null, int? take = null,
        string OrderByDirection = OrderByStrings.Ascending , Expression<Func<T, object>> orderByFunc = null, string[] includes = null)
    {
        IQueryable<T> values = _appDbContext.Set<T>().Where(predicate);

        if (skip.HasValue)
            values = values.Skip<T>(skip.Value);
        if (take.HasValue)
            values = values.Take<T>(take.Value);

        if (orderByFunc != null)
        {
            if (OrderByDirection == OrderByStrings.Ascending)
            values = values.OrderBy(orderByFunc);
            else
                values = values.OrderByDescending(orderByFunc);
        }

        return await values.ToListAsync();
    }


    public async Task Add(T item) => await _appDbContext.Set<T>().AddAsync(item);

    public async Task Update(T item) => _appDbContext.Set<T>().Update(item);

    public async Task Delete(int Id) =>   _appDbContext.Remove(await _appDbContext.FindAsync<T>(Id));

}
