using System;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Users.Application.Services;
using Users.Domain.Models;
using Users.Domain.Repositories;
using Xunit;

namespace UsersUnitTests;

public class UsersServiceGetByEmailUnitTests
{
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly UsersService _service;
    private readonly Fixture _fixture;

    public UsersServiceGetByEmailUnitTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _service = new UsersService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetByEmail_UserExists_ReturnSuccessResult()
    {
        // Arrange
        User expectedUser = _fixture.Create<User>();
        string email = expectedUser.Email;
        _repositoryMock.Setup(repository => repository.GetByEmail(email))
            .ReturnsAsync(Result<User>.Success(expectedUser));

        // Act
        var result = await _service.GetByEmail(email);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedUser, result.Value);
        _repositoryMock.Verify(repository => repository.GetByEmail(email), Times.Once);
    }

    [Fact]
    public async Task GetByEmail_UserDoesNotExist_ReturnNotFoundResult()
    {
        // Arrange
        string email = _fixture.Create<string>();
        string expectedErrorMessage = $"User with email: {email} not found";
        _repositoryMock.Setup(repository => repository.GetByEmail(email))
            .ReturnsAsync(Result<User>.NotFound(expectedErrorMessage));

        // Act
        var result = await _service.GetByEmail(email);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        Assert.Equal(expectedErrorMessage, result.Error.Message);
        _repositoryMock.Verify(repository => repository.GetByEmail(email), Times.Once);
    }
    
    [Fact]
    public async Task GetByEmail_RepositoryFailure_ReturnFailureResult()
    {
        // Arrange
        string email = _fixture.Create<string>();
        string expectedErrorMessage = "Database error";
        _repositoryMock.Setup(repository => repository.GetByEmail(email))
            .ReturnsAsync(Result<User>.Failure(expectedErrorMessage));

        // Act
        var result = await _service.GetByEmail(email);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Unknown, result.Error.Type);
        Assert.Equal(expectedErrorMessage, result.Error.Message);
        _repositoryMock.Verify(repository => repository.GetByEmail(email), Times.Once);
    }
} 