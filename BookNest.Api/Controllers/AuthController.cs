

namespace RepositoryWithUWO.Api.Controllers;

[Route("api/v1/auth")]
[ApiController]
public class AuthController(IAuthServices _userServices, IConfiguration configuration) : ControllerBase
{

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterModel registerModel)
    {
        var result = await _userServices.RegisterUserAsync(registerModel);

        if (!result.IsAuthenticated)
            return BadRequest(result.Message);

        if (result.RefreshToken == null)
            return StatusCode(StatusCodes.Status500InternalServerError, "Refresh token generation failed.");

        SetRefreshTokenInCookies(result.RefreshToken, result.RefreshTokenExpiration);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginModel loginModel)
    {
        var result = await _userServices.LoginAsync(loginModel);

        if (!result.IsAuthenticated)
            return BadRequest(result.Message);

        if (!string.IsNullOrEmpty(result.RefreshToken))
            SetRefreshTokenInCookies(result.RefreshToken, result.RefreshTokenExpiration);

        return Ok(result);
    }

    [HttpPost("AddUserToRole")]
    public async Task<IActionResult> AddUserToRoleAsync(string role, string Username)
    {
        if (!await _userServices.AddUserToRoleAsync(role, Username))
            return BadRequest("لم يتم إضافة المستخدم إلى الدور المحدد");

        return Ok($"{Username} تمت إضافته إلى الدور `{role}`");
    }

    [HttpPost("CreateRole")]
    public async Task<IActionResult> CreateRoleAsync(string role)
    {

        if (!await _userServices.CreateRoleAsync(role))
            return BadRequest("لم يتم إنشاء الدور");

        return Ok($"تم إنشاء الدور `{role}` بنجاح");
    }

    [HttpGet("RefreshToken")]
    public async Task<IActionResult> RefreshToken()
    {
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
        var TokenToRevoke = model.Token ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(TokenToRevoke))
            return BadRequest("الرمز مطلوب!");

        if (!await _userServices.RevokeTokenAsync(TokenToRevoke))
            return BadRequest("الرمز غير صالح أو غير نشط");

        return Ok("تم إلغاء الرمز بنجاح");
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
