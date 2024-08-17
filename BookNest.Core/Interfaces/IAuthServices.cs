using RepositoryWithUOW.Core.Entites;
using RepositoryWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryWithUOW.Core.Interfaces;

public interface IAuthServices //: IBaseRepository<User>
{
    public Task<AuthModel> RegisterUserAsync(RegisterModel model);

    public Task<AuthModel> LoginAsync(LoginModel model);
    public Task<bool> AddUserToRoleAsync(string Role, string Username);
    public Task<bool> CreateRoleAsync(string Role);

    public Task<AuthModel> RefreshTokenAsync(string refreshToken);
    public Task<bool> RevokeTokenAsync(string refreshToken);



}
