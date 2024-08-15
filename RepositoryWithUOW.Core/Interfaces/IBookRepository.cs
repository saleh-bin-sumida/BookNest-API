using RepositoryWithUOW.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryWithUOW.Core.Interfaces
{
    public interface IBookRepository : IBaseRepository<Book>
    {
        public void SpecialMethodForBooks();
    }
}
