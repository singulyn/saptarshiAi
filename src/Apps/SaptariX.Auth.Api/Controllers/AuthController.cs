using Microsoft.AspNetCore.Mvc;
using SaptariX.Auth.Api.Contracts;

namespace SaptariX.Auth.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email and password are required.");
        }

        return Ok(new LoginResponse(
            "jwt-placeholder",
            "refresh-token-placeholder",
            DateTimeOffset.UtcNow.AddMinutes(30)));
    }

    [HttpPost("refresh")]
    public ActionResult<LoginResponse> Refresh(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return BadRequest("Refresh token is required.");
        }

        return Ok(new LoginResponse(
            "jwt-placeholder",
            request.RefreshToken,
            DateTimeOffset.UtcNow.AddMinutes(30)));
    }
}
