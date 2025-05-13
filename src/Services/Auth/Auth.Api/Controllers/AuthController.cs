using Microsoft.AspNetCore.Mvc;
using Auth.Api.Models;
using Auth.Domain.Services;
using Auth.Domain.Models;
using AutoMapper;

namespace Auth.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase {

    private readonly IAuthService _service;
    private readonly IMapper _mapper;

    public AuthController(IAuthService service, IMapper mapper) {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request) {
        UserLogin userLogin = _mapper.Map<UserLogin>(request);

        AuthResult authResult = await _service.Login(userLogin);
        
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

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request) {
        UserRegister user = _mapper.Map<UserRegister>(request);
        AuthResult authResult = await _service.Register(user);

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