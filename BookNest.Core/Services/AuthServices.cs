using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepositoryWithUOW.Core.Entites;
using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUOW.Core.Models;
using RepositoryWithUWO.Core.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;



namespace RepositoryWithUOW.Core.Services;

/// <summary>
/// Service for handling authentication-related operations.
/// </summary>
public class AuthServices : IAuthServices
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtOptions _jwtOptions;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthServices(UserManager<ApplicationUser> userManager, JwtOptions jwtOptions, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _jwtOptions = jwtOptions;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="model">The registration model.</param>
    /// <returns>An authentication model with the registration result.</returns>
    public async Task<AuthModel> RegisterUserAsync(RegisterModel model)
    {
        if (await _userManager.FindByEmailAsync(model.Email) != null)
            return new AuthModel { Message = "Email is already in use" };

        if (await _userManager.FindByNameAsync(model.UserName) != null)
            return new AuthModel { Message = "Username is already in use" };

        if (model.Password != model.ConfirmPassword)
            return new AuthModel { Message = "Confirm password doesn't match password" };

        var user = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.UserName,
            FullName = model.FullName
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return new AuthModel { Message = errors };
        }

        await AddUserToRoleAsync("User", user.UserName);

        var token = await GenerateToken(user);

        return new AuthModel
        {
            Message = "User is created successfully",
            IsAuthenticated = true,
            Username = user.UserName,
            Email = user.Email,
            Roles = new List<string> { "User" },
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = GenerateRefreshToken().Token,
        };
    }

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="model">The login model.</param>
    /// <returns>An authentication model with the login result.</returns>
    public async Task<AuthModel> LoginAsync(LoginModel model)
    {
        var authModel = new AuthModel();

        var user = await _userManager.FindByNameAsync(model.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            return new AuthModel { Message = "Password or username is incorrect!" };

        var token = await GenerateToken(user);

        var activeRefreshToken = user.RefreshTokens?.FirstOrDefault(x => x.IsActive && x.RevokedOn == DateTime.MinValue);
        if (activeRefreshToken != null)
        {
            authModel.RefreshToken = activeRefreshToken.Token;
            authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
        }
        else
        {
            var refreshToken = GenerateRefreshToken();
            authModel.RefreshToken = refreshToken.Token;
            authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
            user.RefreshTokens?.Add(refreshToken);
            await _userManager.UpdateAsync(user);
        }

        authModel.Message = "Correct credentials";
        authModel.IsAuthenticated = true;
        authModel.Username = user.UserName;
        authModel.Email = user.Email;
        authModel.Roles = (await _userManager.GetRolesAsync(user)).ToList();
        authModel.Token = new JwtSecurityTokenHandler().WriteToken(token);
        return authModel;
    }

    /// <summary>
    /// Generates a JWT token for a user.
    /// </summary>
    /// <param name="user">The application user.</param>
    /// <returns>A security token.</returns>
    private async Task<SecurityToken> GenerateToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in await _userManager.GetRolesAsync(user))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issure,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.LifeTime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)), SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims)
        };

        return tokenHandler.CreateToken(tokenDescriptor);
    }

    /// <summary>
    /// Adds a user to a role.
    /// </summary>
    /// <param name="role">The role name.</param>
    /// <param name="username">The username.</param>
    /// <returns>
    /// A boolean indicating success.
    /// Returns false if:
    /// - The user is not found.
    /// - The role does not exist.
    /// - The user is already in the role.
    /// </returns>
    public async Task<bool> AddUserToRoleAsync(string role, string username)
    {
        // if role doesnt esxsit create it
        var user = await _userManager.FindByNameAsync(username);
        if (user == null || !await _roleManager.RoleExistsAsync(role) || await _userManager.IsInRoleAsync(user, role))
            return false;


        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }

        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded;
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="role">The role name.</param>
    /// <returns>A boolean indicating success.</returns>
    public async Task<bool> CreateRoleAsync(string role)
    {
        if (await _roleManager.RoleExistsAsync(role))
            return true;

        var result = await _roleManager.CreateAsync(new IdentityRole(role));
        return result.Succeeded;
    }

    /// <summary>
    /// Generates a new refresh token.
    /// </summary>
    /// <returns>A refresh token.</returns>
    private RefreshToken GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(randomNumber);
        }
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            ExpiresOn = DateTime.UtcNow.AddHours(_jwtOptions.LifeTime),
            CreatedOn = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Refreshes a JWT token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <returns>An authentication model with the refresh result.</returns>
    public async Task<AuthModel> RefreshTokenAsync(string refreshToken)
    {
        var authModel = new AuthModel();

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

        if (user == null)
        {
            authModel.Message = "Invalid token";
            return authModel;
        }

        var existingRefreshToken = user.RefreshTokens.Single(t => t.Token == refreshToken);

        if (!existingRefreshToken.IsActive)
        {
            authModel.Message = "Inactive token";
            return authModel;
        }

        if (existingRefreshToken.RevokedOn != DateTime.MinValue)
        {
            authModel.Message = "Token is revoked, can't be refreshed";
            return authModel;
        }

        existingRefreshToken.RevokedOn = DateTime.UtcNow;
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);

        var jwtToken = await GenerateToken(user);
        authModel.Message = "Refresh token has been refreshed";
        authModel.IsAuthenticated = true;
        authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        authModel.Email = user.Email;
        authModel.Username = user.UserName;
        authModel.Roles = await _userManager.GetRolesAsync(user);
        authModel.RefreshToken = newRefreshToken.Token;
        authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

        return authModel;
    }

    /// <summary>
    /// Revokes a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <returns>A boolean indicating success.</returns>
    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

        if (user == null)
            return false;

        var existingRefreshToken = user.RefreshTokens.Single(t => t.Token == refreshToken);

        if (!existingRefreshToken.IsActive)
            return false;

        existingRefreshToken.RevokedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return true;
    }
}
