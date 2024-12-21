using RepositoryWithUOW.Core.Constants;
using System.Linq.Expressions;

namespace RepositoryWithUOW.Core.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(string[] includes = null);

    Task<T> FindAsync(Expression<Func<T, bool>> predicate, string[] includes = null);
    Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate, string[] includes = null);
    Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate, int? skip = null, int? take = null,
        string OrderByDirection = OrderByStrings.Ascending, Expression<Func<T, object>> orderByFunc = null, string[] includes = null);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T item);
    Task UpdateAsync(T item);
    Task DeleteAsync(int Id);

}
