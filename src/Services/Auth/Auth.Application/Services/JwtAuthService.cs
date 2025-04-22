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

    public JwtAuthService(JwtSettings settings, IPasswordHasher passwordHasher)
    {
        _settings = settings;
        _passwordHasher = passwordHasher;
    }

    public Task<AuthResult> Authenticate(UserDto user, string password)
    {
        if (_passwordHasher.VerifyPassword(password, user.PasswordHash)) {
            return AuthResultCreator.CreateFailed("Invalid credentials");
        }

        if (user.IsActive == false) { 
            return AuthResultCreator.CreateFailed("User is not active");
        }

        string accessToken = GenerateAccessToken(user);

        return AuthResultCreator.CreateSuccess(accessToken, null, _settings.AccessTokenExpirationMinutes * 60);
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

