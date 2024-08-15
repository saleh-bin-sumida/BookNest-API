using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RepositoryWithUOW.Core.Entites;
using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUOW.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RepositoryWithUWO.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(IAuthServices userServices) : ControllerBase
{
    private IAuthServices _userServices  = userServices;



    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterModel registerModel)
    {
        if (!ModelState.IsValid)
            return BadRequest("some proprites are not valid");
        
        var result = await _userServices.RegisterUserAsync(registerModel);

        if (!result.IsAuthenticated)
            return BadRequest(result.Message);

        return Ok(result);

            
    }


    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginModel loginModel)
    {
        if (!ModelState.IsValid)
            return BadRequest("some proprites are not valid");

        var result = await _userServices.LoginAsync(loginModel);

        if (!result.IsAuthenticated)
            return BadRequest(result.Message);

        return Ok(result);
    }


    [HttpPost("AddUserToRole")]
    public async Task<IActionResult> AddUserToRoleAsync(string role, string Username)
    {
        if (!ModelState.IsValid)
            return BadRequest("some proprites are not valid");

        if (!await _userServices.AddUserToRoleAsync(role, Username))
            return BadRequest("User didnt added to the provided role");

        return Ok($"{Username} add to the role `{role}`");
    }

    [HttpPost("CreateRole")]
    public async Task<IActionResult> CreateRoleAsync(string role)
    {
        if (!ModelState.IsValid)
            return BadRequest("some proprites are not valid");

        if (!await _userServices.CreateRoleAsync(role))
            return BadRequest("role did not create");

        return Ok($"`{role}` role Created Succesfully");
    }



}
