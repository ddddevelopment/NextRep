using Microsoft.AspNetCore.Mvc;
using Auth.Api.Models;
using Auth.Domain.Services;
using Auth.Domain.Models;
using System.Threading.Tasks;
using Auth.Application.Services;
namespace Auth.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase {

    private readonly IAuthService _service;
    private readonly IUserServiceClient _userServiceClient;

    public AuthController(IAuthService service, IUserServiceClient userServiceClient) {
        _service = service;
        _userServiceClient = userServiceClient;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request) {
        UserDto user = await _userServiceClient.GetUserByEmail(request.Email);

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