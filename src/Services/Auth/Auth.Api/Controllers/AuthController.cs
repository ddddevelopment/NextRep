using Auth.Domain.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase {
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginRequest request) {
        var token = await _service.GenerateToken(request.Email);
        return Ok(token);
    }
}