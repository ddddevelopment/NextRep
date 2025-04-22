using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Domain.Models;
using Auth.Domain.Repositories;
using Auth.Domain.Services;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Application.Services;

public class JwtAuthService : IAuthService
{
    private readonly JwtSettings _settings;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserServiceClient _userServiceClient;
    
    public JwtAuthService(JwtSettings settings, IRefreshTokenRepository refreshTokenRepository, IPasswordHasher passwordHasher, IUserServiceClient userServiceClient)
    {
        _settings = settings;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _userServiceClient = userServiceClient;
    }

    public async Task<AuthResult> Authenticate(UserDto user, string password)
    {
        if (_passwordHasher.VerifyPassword(password, user.PasswordHash) == false) {
            return AuthResult.CreateFailed("Email or password are not correct");
        }

        if (user.IsActive == false) {
            return AuthResult.CreateFailed("User is not active");
        }

        string accessToken = GenerateAccessToken(user);
        string refreshToken = GenerateRefreshToken();

        await _refreshTokenRepository.SaveToken(new RefreshToken {
            Token = refreshToken,
            UserId = user.Id,
            CreatedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays),
            Used = false,
            Revoked = false
        });

        return AuthResult.CreateSuccess(accessToken, refreshToken, _settings.AccessTokenExpiryMinutes * 60);
    }


    public async Task<AuthResult> RefreshToken(string refreshToken)
    {
        RefreshToken storedToken = await _refreshTokenRepository.GetToken(refreshToken);

        if (storedToken == null || storedToken.Used || storedToken.Revoked || storedToken.ExpiryDate <= DateTime.UtcNow) {
            return AuthResult.CreateFailed("Invalid or expired refresh token");
        }

        
        if (user == null) {
            return AuthResult.CreateFailed("User not found");
        }

        if (user.IsActive == false) {
            return AuthResult.CreateFailed("User is not active");
        }

        await _refreshTokenRepository.RevokeToken(refreshToken);

        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();

        await _refreshTokenRepository.SaveToken(new RefreshToken {
            Token = newRefreshToken,
            UserId = user.Id,
            CreatedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays),
            Used = false,
            Revoked = false
        });

        return AuthResult.CreateSuccess(newAccessToken, newRefreshToken, _settings.AccessTokenExpiryMinutes * 60);
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        await _refreshTokenRepository.RevokeToken(refreshToken);
    }

    public async Task<Auth.Domain.Models.TokenValidationResult> ValidateToken(string token)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(_settings.Secret);

        try {
            var validationParameters = new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out _);

            string userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var claims = new Dictionary<string, string>();
            foreach (var claim in principal.Claims) {
                claims[claim.Type] = claim.Value;
            }

            return new Auth.Domain.Models.TokenValidationResult { Valid = true, UserId = Guid.Parse(userId), Claims = claims };
        }
        catch (Exception exception) {
            Console.WriteLine($"Token validation error: {exception.Message}");
            return new Domain.Models.TokenValidationResult { Valid = false };
        }
    }

    private string GenerateAccessToken(UserDto user) {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(_settings.Secret);

        List<Claim> claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.Roles) {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpiryMinutes),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
        string token = tokenHandler.WriteToken(securityToken);

        return token;
    }

    private string GenerateRefreshToken() {
        var randomBytes = new byte[32];
        using var rngCryptoServiceProvider = RandomNumberGenerator.Create();

        rngCryptoServiceProvider.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }
}