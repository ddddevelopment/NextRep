using System.Threading.Tasks;
using Auth.Application.Services;
using Auth.Domain.Models;
using Auth.Domain.Services;
using AutoFixture;
using Moq;

namespace JwtAuthServiceUnitTests;

public class JwtAuthServiceAuthenticateUnitTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly JwtAuthService _service;
    private readonly JwtSettings _settings;

    public JwtAuthServiceAuthenticateUnitTests()
    {
        _fixture = new Fixture();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _settings = _fixture.Create<JwtSettings>();
        _service = new JwtAuthService(_settings, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Authenticate_ValidCredentials_ReturnSuccessAuthResult()
    {
        UserDto user = _fixture.Build<UserDto>().With(user => user.IsActive, true).Create();
        string password = "valid_password";
        _passwordHasherMock.Setup(passwordHasher => passwordHasher.VerifyPassword(password, user.PasswordHash)).Returns(true);

        AuthResult result = await _service.Authenticate(user, password);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.AccessToken);
        Assert.Equal(_settings.AccessTokenExpirationMinutes * 60, result.ExpiresIn);
    }

    [Fact]
    public async Task Authenticate_InvalidCredentials_ReturnFailedAuthResult()
    {
        UserDto user = _fixture.Create<UserDto>();
        string password = "invalid_password";
        _passwordHasherMock.Setup(passwordHasher => passwordHasher.VerifyPassword(password, user.PasswordHash)).Returns(false);
        
        AuthResult result = await _service.Authenticate(user, password);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.ErrorMessage);
    }

    [Fact]
    public async Task Authenticate_InactiveUser_ReturnFailedAuthResult() {
        UserDto user = _fixture.Build<UserDto>().With(user => user.IsActive, false).Create();
        string password = "valid_password";
        _passwordHasherMock.Setup(passwordHasher => passwordHasher.VerifyPassword(password, user.PasswordHash)).Returns(true);

        AuthResult result = await _service.Authenticate(user, password);

        Assert.False(result.IsSuccess);
        Assert.Equal("User is not active", result.ErrorMessage);
    }
}
