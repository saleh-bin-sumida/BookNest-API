using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryWithUWO.EF.Repositories;

public class BookRepository : BaseRepository<Book>, IBookRepository
{
    public BookRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public void SpecialMethodForBooks()
    {
       
    }
}
