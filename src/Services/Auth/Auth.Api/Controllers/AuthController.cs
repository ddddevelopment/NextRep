using Auth.Api.Models;
using Auth.Domain.Models;
using Auth.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    private readonly IUserServiceClient _userServiceClient;

    public AuthController(IAuthService service, IUserServiceClient userServiceClient)
    {
        _authService = service;
        _userServiceClient = userServiceClient;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginRequest request)
    {
        if (ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        UserDto user = await _userServiceClient.GetUserByEmail(request.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Email or password are not correct" });
        }

        var authResult = await _authService.Authenticate(user, request.Password);

        if (authResult.Success == false)
        {
            return Unauthorized(new { message = authResult.ErrorMessage });
        }

        return Ok(new
        {
            accessToken = authResult.AccessToken,
            refreshToken = authResult.RefreshToken,
            expiresIn = authResult.ExpiresIn
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(RefreshTokenRequest request) {
        if (string.IsNullOrEmpty(request.RefreshToken)) {
            return BadRequest(new { message = "Refresh token is required" });
        }
        
        var result = await _authService.RefreshToken(request.RefreshToken);

        if (result.Success == false) {
            return Unauthorized(new { message = result.ErrorMessage });
        }

        return Ok(new {
            accessToken = result.AccessToken,
            refreshToken = result.RefreshToken,
            expiresIn = result.ExpiresIn
        });
    }

    [HttpPost("validate")]
    public async Task<ActionResult> ValidateToken(ValidateTokenRequest request) {
        if (string.IsNullOrEmpty(request.Token)) {
            return BadRequest(new { message = "Token is required" });
        }

        TokenValidationResult result = await _authService.ValidateToken(request.Token);

        if (result.Valid == false) {
            return Unauthorized(new { message = "Invalid token" });
        }

        return Ok(new {
            isValid = true,
            userId = result.UserId,
            claims = result.Claims
        });
    }
}