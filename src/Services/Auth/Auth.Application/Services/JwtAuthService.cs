using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Domain.Models;
using Auth.Domain.Services;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Application.Services;

public class JwtAuthService : IAuthService
{
    private readonly JwtSettings _settings;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUsersServiceClient _usersServiceClient;
    public JwtAuthService(JwtSettings settings, IPasswordHasher passwordHasher, IUsersServiceClient usersServiceClient)
    {
        _settings = settings;
        _passwordHasher = passwordHasher;
        _usersServiceClient = usersServiceClient;
    }

    public async Task<AuthResult> Login(UserLogin login)
    {
        UserDto user = await _usersServiceClient.GetUserByEmail(login.Email);
        if (user == null) {
            return AuthResultCreator.CreateFailed($"User with email: {login.Email} not exists");
        }

        bool isAuthenticated = Authenticate(user, login.Password);
        if (isAuthenticated == false) {
            return AuthResultCreator.CreateFailed("Invalid credentials");
        }
        
        string accessToken = GenerateAccessToken(user);
        return AuthResultCreator.CreateSuccess(accessToken, null, _settings.AccessTokenExpirationMinutes * 60);
    }

    private bool Authenticate(UserDto user, string password) => _passwordHasher.VerifyPassword(password, user.PasswordHash)

    public async Task<AuthResult> Register(UserRegister user)
    {
        bool userExists = await _usersServiceClient.GetUserByEmail(user.Email) != null;
        if (userExists) {
            return new AuthResult() { IsSuccess = false, ErrorMessage = "User already exists" };
        }

        string passwordHash = _passwordHasher.HashPassword(user.Password);

        
    }

    private string GenerateAccessToken(UserDto user) {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(_settings.SecretKey);
        List<Claim> claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.Roles) {
            claims.Add(new Claim(ClaimTypes.Role, ((int)role).ToString()));
        }

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
        string token = tokenHandler.WriteToken(securityToken);

        return token;
    }
}

