using Auth.Application.Services;
using Auth.Domain.Models;
using Auth.Domain.Services;
using AutoFixture;
using AutoMapper;
using Moq;

namespace Auth.Tests.JwtAuthServiceUnitTests;

public class JwtAuthServiceRegisterUnitTests
{
    private readonly JwtAuthService _service;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IUsersServiceClient> _usersServiceClientMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly JwtSettings _jwtSettings;
    private readonly Fixture _fixture;

    public JwtAuthServiceRegisterUnitTests()
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
    public async Task Register_UserDoesNotExist_ReturnsSuccessResult()
    {
        // Arrange
        var register = _fixture.Create<UserRegister>();
        var user = _fixture.Build<UserDto>()
            .With(u => u.Email, register.Email)
            .With(u => u.PasswordHash, "hashed")
            .Create();
        _usersServiceClientMock.Setup(x => x.GetUserByEmail(register.Email))
            .ReturnsAsync(UserGetResult.NotFound());
        _passwordHasherMock.Setup(x => x.HashPassword(register.Password))
            .Returns("hashed");
        _mapperMock.Setup(m => m.Map<UserDto>(
            It.Is<UserRegister>(r => r.Email == register.Email), 
            It.IsAny<Action<IMappingOperationOptions<object, UserDto>>>()))
            .Returns(user);
        _usersServiceClientMock.Setup(x => x.CreateUser(user))
            .ReturnsAsync(UserCreateResult.Success());

        // Act
        var result = await _service.Register(register);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
        Assert.Equal(_jwtSettings.AccessTokenExpirationMinutes * 60, result.ExpiresIn);
        _usersServiceClientMock.Verify(x => x.GetUserByEmail(register.Email), Times.Once);
        _passwordHasherMock.Verify(x => x.HashPassword(register.Password), Times.Once);
        _mapperMock.Verify(m => m.Map<UserDto>(
            It.Is<UserRegister>(r => r.Email == register.Email), 
            It.IsAny<Action<IMappingOperationOptions<object, UserDto>>>()), Times.Once);
        _usersServiceClientMock.Verify(x => x.CreateUser(user), Times.Once);
    }

    [Fact]
    public async Task Register_UserAlreadyExists_ReturnsFailureResult()
    {
        // Arrange
        var register = _fixture.Create<UserRegister>();
        _usersServiceClientMock.Setup(x => x.GetUserByEmail(register.Email))
            .ReturnsAsync(UserGetResult.Found(_fixture.Create<UserDto>()));

        // Act
        var result = await _service.Register(register);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User already exists", result.ErrorMessage);
        _usersServiceClientMock.Verify(x => x.GetUserByEmail(register.Email), Times.Once);
        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _mapperMock.Verify(m => m.Map<UserDto>(It.IsAny<UserRegister>(), It.IsAny<Action<IMappingOperationOptions>>()), Times.Never);
        _usersServiceClientMock.Verify(x => x.CreateUser(It.IsAny<UserDto>()), Times.Never);
    }

    [Fact]
    public async Task Register_UserCreationFails_ReturnsFailureResult()
    {
        // Arrange
        var register = _fixture.Create<UserRegister>();
        var user = _fixture.Build<UserDto>()
            .With(u => u.Email, register.Email)
            .With(u => u.PasswordHash, "hashed")
            .Create();
        _usersServiceClientMock.Setup(x => x.GetUserByEmail(register.Email))
            .ReturnsAsync(UserGetResult.NotFound());
        _passwordHasherMock.Setup(x => x.HashPassword(register.Password))
            .Returns("hashed");
        _mapperMock.Setup(m => m.Map<UserDto>(
            It.Is<UserRegister>(r => r.Email == register.Email), 
            It.IsAny<Action<IMappingOperationOptions<object, UserDto>>>()))
            .Returns(user);
        _usersServiceClientMock.Setup(x => x.CreateUser(user))
            .ReturnsAsync(UserCreateResult.Failure("Failed to create user"));

        // Act
        var result = await _service.Register(register);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Failed to create user", result.ErrorMessage);
        _usersServiceClientMock.Verify(x => x.GetUserByEmail(register.Email), Times.Once);
        _passwordHasherMock.Verify(x => x.HashPassword(register.Password), Times.Once);
        _mapperMock.Verify(m => m.Map<UserDto>(
            It.Is<UserRegister>(r => r.Email == register.Email), 
            It.IsAny<Action<IMappingOperationOptions<object, UserDto>>>()), Times.Once);
        _usersServiceClientMock.Verify(x => x.CreateUser(user), Times.Once);
    }
}