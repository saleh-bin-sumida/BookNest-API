using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RepositoryWithUOW.Core.Entites;
using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUOW.Core.Models;
using RepositoryWithUWO.Core.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;



namespace RepositoryWithUOW.Core.Services;

public class AuthServices
    (
    UserManager<ApplicationUser> userManager,
    JwtOptions jwt,
    RoleManager<IdentityRole> roleManager
    ) : IAuthServices
{
    public UserManager<ApplicationUser> _userManager = userManager;
    private readonly JwtOptions _jwtOptions = jwt;

    public async Task<AuthModel> RegisterUserAsync(RegisterModel model)
    {
        if (await _userManager.FindByEmailAsync(model.Email) is not null)
            return new AuthModel { Message = "email is already in use" };

        if (await _userManager.FindByNameAsync(model.Email) is not null)
            return new AuthModel { Message = "username is already in use" };

        if (model.Password != model.ConfirmPassword)
            return new AuthModel { Message = "confirm password doesnt match password" };

        var User = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.UserName,
            FullName = model.FullName
        };

        var result = await (_userManager.CreateAsync(User, model.Password));

        if (!result.Succeeded)
        {
            var Errors = string.Empty;
            foreach (var error in result.Errors.Select(e => e.Description))
                Errors += " " + error;

            return new AuthModel { Message = Errors };
        }

        _ = await AddUserToRoleAsync("User", User.UserName);

        var token = await GenerateToken(User);

        return new AuthModel
        {
            Message = "User is created sussefsully",
            IsAuthenticated = true,
            Username = User.UserName,
            Email = User.Email,
            Roles = new List<string> { "User" },
            Token = new JwtSecurityTokenHandler().WriteToken(token),
        };

    }



    public async Task<AuthModel> LoginAsync(LoginModel model)
    {
        AuthModel authModel = new();

        var User = await _userManager.FindByNameAsync(model.Username);

        if (User is null || !(await _userManager.CheckPasswordAsync(User, model.Password)))
            return new AuthModel { Message = "password or username is incorrect!" };


        var token = await GenerateToken(User);

        // checking if user has active refresh token
        var ActiveRefreshToken = User.RefreshTokens.FirstOrDefault(x => x.IsActive && x.RevokedOn == DateTime.MinValue);
        if (ActiveRefreshToken is not null)
        {
            authModel.RefreshToken = ActiveRefreshToken.Token;
            authModel.RefreshTokenExpiration = ActiveRefreshToken.ExpiresOn;
        }
        else
        {
            var refreshToken = GenerateRefrsshToken();
            authModel.RefreshToken = refreshToken.Token;
            authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
            User.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(User);
        }


        authModel.Message = "correct Credential";
        authModel.IsAuthenticated = true;
        authModel.Username = User.UserName;
        authModel.Email = User.Email;
        authModel.Roles = (await _userManager.GetRolesAsync(User)).ToList();
        authModel.Token = new JwtSecurityTokenHandler().WriteToken(token);
        return authModel;
    }



    private async Task<SecurityToken> GenerateToken(ApplicationUser user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var item in await _userManager.GetRolesAsync(user))
            claims.Add(new Claim(ClaimTypes.Role, item));

        var TokenHandler = new JwtSecurityTokenHandler();
        var TokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issure,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.Now.AddSeconds(_jwtOptions.LifeTime),
            SigningCredentials =
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)),
            SecurityAlgorithms.HmacSha256),

            Subject = new ClaimsIdentity(claims)
        };

        //var AccessToken = TokenHandler.WriteToken(securityToken);
        //return AccessToken;
        return TokenHandler.CreateToken(TokenDescriptor);

    }

    public async Task<bool> AddUserToRoleAsync(string Role, string Username)
    {
        var User = await _userManager.FindByNameAsync(Username);
        if (User is null)
            return false;



        if (!await roleManager.RoleExistsAsync(Role))
            return false;

        if (await _userManager.IsInRoleAsync(User, Role))
            return false;

        var result = await _userManager.AddToRoleAsync(User, Role);
        return result.Succeeded;
    }

    public async Task<bool> CreateRoleAsync(string Role)
    {
        var NewRole = new IdentityRole(Role);
        var result = await roleManager.CreateAsync(NewRole);
        return result.Succeeded;
    }

    private RefreshToken GenerateRefrsshToken()
    {
        var RandomNumber = new byte[32];
        using (var genrator = RandomNumberGenerator.Create())
        {
            genrator.GetBytes(RandomNumber);
        }
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumber),
            ExpiresOn = DateTime.UtcNow.AddHours(_jwtOptions.LifeTime),
            CreatedOn = DateTime.UtcNow
        };
    }

    public async Task<AuthModel> RefreshTokenAsync(string refreshToken)
    {
        AuthModel authModel = new();

        var User = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

        if (User is null)
        {
            authModel.Message = "invalid Token";
            return authModel;
        }
            
        var ExsistingRefreshToken = User.RefreshTokens.Single(t => t.Token == refreshToken);

        if (!ExsistingRefreshToken.IsActive)
        {
            authModel.Message = "Inactive Token";
            return authModel;
        }

        if (ExsistingRefreshToken.RevokedOn != DateTime.MinValue)
        {
            authModel.Message = "Token is revoked, cant be refrshed";
            return authModel;
        }

        ExsistingRefreshToken.RevokedOn = DateTime.UtcNow;
        var NewRefreshToken = GenerateRefrsshToken();

        User.RefreshTokens.Add(NewRefreshToken);
        await _userManager.UpdateAsync(User);

        var JwtToken = await GenerateToken(User);
        authModel.Message = "refresh token has been refreshed"; 
        authModel.IsAuthenticated = true;
        authModel.Token = new JwtSecurityTokenHandler().WriteToken(JwtToken);
        authModel.Email = User.Email;
        authModel.Username = User.UserName;
        authModel.Roles = await _userManager.GetRolesAsync(User);
        authModel.RefreshToken = NewRefreshToken.Token;
        authModel.RefreshTokenExpiration = NewRefreshToken.ExpiresOn;
        
        return authModel;
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        AuthModel authModel = new();

        var User = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

        if (User is null)
            return false;

        var ExsistingRefreshToken = User.RefreshTokens.Single(t => t.Token == refreshToken);

        if (!ExsistingRefreshToken.IsActive)
            return false;

        ExsistingRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(User);

        return true;
    }
}
