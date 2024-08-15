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
        if (model is null)
            throw new NullReferenceException("the register model is null");

        if( await _userManager.FindByEmailAsync(model.Email) is not null)
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

            return new AuthModel {Message = Errors};
        }

        _ = await AddUserToRoleAsync("User", User.UserName);

        var token = await GenerateToken(User);

        return new AuthModel
        {
            Message = "User is created sussefsully",
            IsAuthenticated = true,
            ExpirsOn = token.ValidTo,
            Username = User.UserName,
            Email = User.Email,
            Roles = new List<string> { "User" },
            Token = new JwtSecurityTokenHandler().WriteToken(token),
        };

    }

  

    public async Task<AuthModel> LoginAsync(LoginModel model)
    {
        if (model is null)
            throw new NullReferenceException("the login model is null");

        var User = await _userManager.FindByNameAsync(model.Username);

        if (User is null)
            return new AuthModel { Message = "no user with that username" };

        var IsUser = await _userManager.CheckPasswordAsync(User, model.Password);

        if (!IsUser)
            return new AuthModel { Message = "invalid password" };

        var token = await GenerateToken(User);

        return new AuthModel
        {
            Message = "correct Credential",
            IsAuthenticated = true,
            ExpirsOn = token.ValidTo,
            Username = User.UserName,
            Email = User.Email,
            Roles = await _userManager.GetRolesAsync(User),
            Token = new JwtSecurityTokenHandler().WriteToken(token),
        };
    }



    private async Task<SecurityToken> GenerateToken(ApplicationUser user)
    {
        var TokenHandler = new JwtSecurityTokenHandler();
        var TokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issure,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.Now.AddMinutes(_jwtOptions.LifeTime),
            SigningCredentials =
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)),
            SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim (ClaimTypes.Name, user.UserName),
                new Claim (ClaimTypes.Email, user.Email),
            })
        };

        //var AccessToken = TokenHandler.WriteToken(securityToken);
        //return AccessToken;
        return  TokenHandler.CreateToken(TokenDescriptor);

    }

    public async Task<bool> AddUserToRoleAsync(string Role, string Username)
    {
        var User = await _userManager.FindByNameAsync(Username);
        if (User is null)
            return false;


        if (!await roleManager.RoleExistsAsync(Role))
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

    
}
