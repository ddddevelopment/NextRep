using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoFixture;
using AutoMapper;
using Moq;
using Xunit;
using Auth.Application.Services;
using Auth.Domain.Models;
using Auth.Domain.Services;

namespace Auth.Tests.JwtAuthServiceUnitTests;

public class JwtAuthServiceLoginUnitTests
{
    private readonly JwtAuthService _service;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IUsersServiceClient> _usersServiceClientMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly JwtSettings _jwtSettings;
    private readonly Fixture _fixture;

    public JwtAuthServiceLoginUnitTests()
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _usersServiceClientMock = new Mock<IUsersServiceClient>();
        _mapperMock = new Mock<IMapper>();
        _jwtSettings = new JwtSettings
        {
            SecretKey = "test_secret_key_1234567890_longer_key_to_have_at_least_32_bytes",
            AccessTokenExpirationMinutes = 60,
            Issuer = "TestIssuer",
            Audience = "TestAudience"
        };
        _service = new JwtAuthService(_jwtSettings, _passwordHasherMock.Object, _usersServiceClientMock.Object, _mapperMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Login_UserExistsAndPasswordValid_ReturnsSuccessResult()
    {
        // Arrange
        var login = _fixture.Create<UserLogin>();
        var user = _fixture.Build<UserDto>()
            .With(u => u.Email, login.Email)
            .With(u => u.PasswordHash, "hashed")
            .Create();
        _usersServiceClientMock.Setup(x => x.GetUserByEmail(login.Email))
            .ReturnsAsync(UserGetResult.Found(user));
        _passwordHasherMock.Setup(x => x.VerifyPassword(login.Password, user.PasswordHash))
            .Returns(true);

        // Act
        var result = await _service.Login(login);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
        Assert.Equal(_jwtSettings.AccessTokenExpirationMinutes * 60, result.ExpiresIn);
        _usersServiceClientMock.Verify(x => x.GetUserByEmail(login.Email), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(login.Password, user.PasswordHash), Times.Once);
    }

    [Fact]
    public async Task Login_UserDoesNotFound_ReturnsFailureResult()
    {
        // Arrange
        var login = _fixture.Create<UserLogin>();
        _usersServiceClientMock.Setup(x => x.GetUserByEmail(login.Email))
            .ReturnsAsync(UserGetResult.NotFound());

        // Act
        var result = await _service.Login(login);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.ErrorMessage);
        _usersServiceClientMock.Verify(x => x.GetUserByEmail(login.Email), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsFailureResult()
    {
        // Arrange
        var login = _fixture.Create<UserLogin>();
        var user = _fixture.Build<UserDto>()
            .With(u => u.Email, login.Email)
            .With(u => u.PasswordHash, "hashed")
            .Create();
        _usersServiceClientMock.Setup(x => x.GetUserByEmail(login.Email))
            .ReturnsAsync(UserGetResult.Found(user));
        _passwordHasherMock.Setup(x => x.VerifyPassword(login.Password, user.PasswordHash))
            .Returns(false);

        // Act
        var result = await _service.Login(login);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.ErrorMessage);
        _usersServiceClientMock.Verify(x => x.GetUserByEmail(login.Email), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(login.Password, user.PasswordHash), Times.Once);
    }
}