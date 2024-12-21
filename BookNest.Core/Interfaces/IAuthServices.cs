using RepositoryWithUOW.Core.Models;

namespace RepositoryWithUOW.Core.Interfaces;

public interface IAuthServices
{
    public Task<AuthModel> RegisterUserAsync(RegisterModel model);

    public Task<AuthModel> LoginAsync(LoginModel model);
    public Task<bool> AddUserToRoleAsync(string Role, string Username);
    public Task<bool> CreateRoleAsync(string Role);

    public Task<AuthModel> RefreshTokenAsync(string refreshToken);
    public Task<bool> RevokeTokenAsync(string refreshToken);



}
