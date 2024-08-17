using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUOW.Core.Models;


namespace RepositoryWithUWO.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(IAuthServices userServices) : ControllerBase
{
    private IAuthServices _userServices  = userServices;


    [HttpGet("test")]
    [Authorize]
    public async Task<IActionResult> get()
    {
        return Ok("yes here we go");
    }




    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterModel registerModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var result = await _userServices.RegisterUserAsync(registerModel);

        if (!result.IsAuthenticated)
            return BadRequest(result.Message);

        SetRefreshTokenInCookies(result.RefreshToken, result.RefreshTokenExpiration);
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

        if (!string.IsNullOrEmpty(result.RefreshToken))
            SetRefreshTokenInCookies(result.RefreshToken,result.RefreshTokenExpiration);

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




    [HttpGet("RefreshToken")]
    public async Task<IActionResult> RefreshToken()
    {
        if (!ModelState.IsValid)
            return BadRequest("some proprites are not valid");

        var refreshToken = Request.Cookies["refreshToken"];

        var result = await _userServices.RefreshTokenAsync(refreshToken);

        if (!result.IsAuthenticated)
            return BadRequest(result);


        if (!string.IsNullOrEmpty(result.RefreshToken))
            SetRefreshTokenInCookies(result.RefreshToken, result.RefreshTokenExpiration);

        return Ok(result);
    }




    [HttpPost("RevokeToken")]
    public async Task<IActionResult> RevokeTokenAsync([FromBody] RevokeToken model)
    {
        if (!ModelState.IsValid)
            return BadRequest("some proprites are not valid");

        var TokenToRevoke = model.Token ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(TokenToRevoke))
            return BadRequest("token is required!");


        if (!await _userServices.RevokeTokenAsync(TokenToRevoke))
            return BadRequest("token invalid or inactive");

        return Ok("token revoked sussecfully");
    }




    private void SetRefreshTokenInCookies(string refreshToken, DateTime ExpirsOn)
    {
        var cookiesOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = ExpirsOn.ToLocalTime(),
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookiesOptions);
    }
}
