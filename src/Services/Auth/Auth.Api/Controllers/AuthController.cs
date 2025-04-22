using Microsoft.AspNetCore.Mvc;
using Auth.Api.Models;
using Auth.Domain.Services;
using Auth.Domain.Models;
using System.Threading.Tasks;
namespace Auth.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase {

    private readonly IAuthService _service;

    public AuthController(IAuthService service) {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request) {
        UserDto user = new UserDto {
            Email = request.Email,
            PasswordHash = request.Password
        };

        AuthResult authResult = await _service.Authenticate(user, request.Password);

        if (authResult.IsSuccess == false) {
            return Unauthorized(new AuthResponse {
                ErrorMessage = "Invalid credentials"
            });
        }

        return Ok(new AuthResponse() {
            AccessToken = authResult.AccessToken,
            ExpiresIn = authResult.ExpiresIn
        });
    }
}