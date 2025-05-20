using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Domain.Models;
using Auth.Domain.Services;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Application.Services;

public class JwtAuthService : IAuthService
{
    private readonly JwtSettings _settings;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUsersServiceClient _usersServiceClient;
    private readonly IMapper _mapper;

    public JwtAuthService(JwtSettings settings, IPasswordHasher passwordHasher, IUsersServiceClient usersServiceClient, IMapper mapper)
    {
        _settings = settings;
        _passwordHasher = passwordHasher;
        _usersServiceClient = usersServiceClient;
        _mapper = mapper;
    }

    public async Task<AuthResult> Login(UserLogin login)
    {
        UserGetResult userGetResult = await _usersServiceClient.GetUserByEmail(login.Email);
        if (userGetResult.IsSuccess == false)
        {
            return AuthResult.Failure($"Error occurred while getting user: {userGetResult.ErrorMessage}");
        }

        if (userGetResult.IsFound == false)
        {
            return AuthResult.Failure($"User with email: {login.Email} not found");
        }

        UserDto user = userGetResult.User;
        bool isAuthenticated = Authenticate(user, login.Password);
        if (isAuthenticated == false)
        {
            return AuthResult.Failure("Invalid credentials");
        }

        string accessToken = GenerateAccessToken(user);
        return AuthResult.Success(accessToken, null, _settings.AccessTokenExpirationMinutes * 60);
    }

    private bool Authenticate(UserDto user, string password) => _passwordHasher.VerifyPassword(password, user.PasswordHash);

    public async Task<AuthResult> Register(UserRegister register)
    {
        UserGetResult userGetResult = await _usersServiceClient.GetUserByEmail(register.Email);

        if (userGetResult.IsSuccess == false)
        {
            return AuthResult.Failure($"Error occurred while getting user: {userGetResult.ErrorMessage}");
        }

        if (userGetResult.IsFound)
        {
            return AuthResult.Failure("User already exists");
        }

        string passwordHash = _passwordHasher.HashPassword(register.Password);

        UserDto user = _mapper.Map<UserDto>(register, opt => opt.AfterMap((src, dest) => dest.PasswordHash = passwordHash));

        UserCreateResult userCreateResult = await _usersServiceClient.CreateUser(user);

        if (userCreateResult.IsSuccess == false)
        {
            return AuthResult.Failure($"Error occured while creating user: {userCreateResult.ErrorMessage}");
        }

        string accessToken = GenerateAccessToken(user);
        return AuthResult.Success(accessToken, null, _settings.AccessTokenExpirationMinutes * 60);
    }

    private string GenerateAccessToken(UserDto user)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(_settings.SecretKey);
        List<Claim> claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
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

