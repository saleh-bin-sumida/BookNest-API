using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUOW.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RepositoryWithUWO.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(IUserServices userServices, JwtOptions jwtOptions) : ControllerBase
{
    private IUserServices _userServices  = userServices;



    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterModel registerModel)
    {
        if (!ModelState.IsValid)
            return BadRequest("some proprites are not valid");
        
        var result = await _userServices.RegisterUserAsync(registerModel);

        if (result.IsSucced)
            return Ok(result);

        return BadRequest(result.Errors.FirstOrDefault());

            
    }


    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginModel loginModel)
    {
        if (!ModelState.IsValid)
            return BadRequest("some proprites are not valid");

        var result = await _userServices.Login(loginModel);

        if (result.IsSucced)
            return Ok( GenerateToken(loginModel));

        return BadRequest("incorrect usrname or password");
    }



    private string GenerateToken(LoginModel login)
    {
        var TokenHandler = new JwtSecurityTokenHandler();
        var TokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtOptions.Issure,
            Audience = jwtOptions.Audience,
            SigningCredentials = 
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim (ClaimTypes.Name, login.Username),
            })
        };

        var securityToken = TokenHandler.CreateToken(TokenDescriptor);
        var AccessToken = TokenHandler.WriteToken(securityToken);
        return AccessToken;

    }

}
