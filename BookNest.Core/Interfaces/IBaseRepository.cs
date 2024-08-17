using RepositoryWithUOW.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryWithUOW.Core.Interfaces;

public interface IBaseRepository<T> where T : class
{
    //T GetById(int id);
    //IEnumerable<T> Index();

   // Task<T> GetByIdAsync(int id, string[] includes = null);
    Task< IEnumerable<T>> IndexAsync(string[] includes = null); 

    Task<T> Find(Expression< Func<T, bool>> predicate, string[] includes = null);
    Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate, string[] includes = null);
    Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate,int? skip = null, int? take = null,
        string OrderByDirection = OrderByStrings.Ascending ,Expression<Func<T,object>> orderByFunc = null, string[] includes = null);

    Task Add(T item);
    
    Task Update(T item);
    Task Delete(int Id);

}
