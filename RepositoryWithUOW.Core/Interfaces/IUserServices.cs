using RepositoryWithUOW.Core.Entites;
using RepositoryWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryWithUOW.Core.Interfaces;

public interface IUserServices //: IBaseRepository<User>
{
    public Task<UserManagerResponse> RegisterUserAsync(RegisterModel model);
    public Task<UserManagerResponse> Login(LoginModel model);

}
