using AutoFixture;
using Moq;
using Users.Application.Services;
using Users.Domain.Models;
using Users.Domain.Repositories;

namespace UsersUnitTests;

public class UsersServiceCreateUnitTests
{
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly UsersService _service;
    private readonly IFixture _fixture;

    public UsersServiceCreateUnitTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _service = new UsersService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Create_ValidUserAndNotExistent_ReturnSuccessResult()
    {
        // Arrange
        var user = _fixture.Create<User>();
        _repositoryMock.Setup(repo => repo.ExistsByEmail(user.Email)).ReturnsAsync(false);
        _repositoryMock.Setup(repo => repo.Add(user)).ReturnsAsync(Result.Success());

        // Act
        var result = await _service.Create(user);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(repo => repo.ExistsByEmail(user.Email), Times.Once);
        _repositoryMock.Verify(repo => repo.Add(user), Times.Once);
    }

    [Fact]
    public async Task Create_NullUser_ReturnInvalidResult()
    {
        // Arrange
        User user = null;

        // Act
        var result = await _service.Create(user);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        _repositoryMock.Verify(repo => repo.ExistsByEmail(It.IsAny<string>()), Times.Never);
        _repositoryMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Create_UserWithExistingEmail_ReturnConflictResult()
    {
        // Arrange
        var user = _fixture.Create<User>();
        _repositoryMock.Setup(repo => repo.ExistsByEmail(user.Email)).ReturnsAsync(true);

        // Act
        var result = await _service.Create(user);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
        _repositoryMock.Verify(repo => repo.ExistsByEmail(user.Email), Times.Once);
        _repositoryMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Create_RepositoryFailure_ReturnFailureResult()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var expectedErrorMessage = "Database error";
        
        _repositoryMock.Setup(repo => repo.ExistsByEmail(user.Email)).ReturnsAsync(false);
        _repositoryMock.Setup(repo => repo.Add(user)).ReturnsAsync(Result.Failure(expectedErrorMessage));

        // Act
        var result = await _service.Create(user);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Unknown, result.Error.Type);
        Assert.Equal(expectedErrorMessage, result.Error.Message);
        _repositoryMock.Verify(repo => repo.ExistsByEmail(user.Email), Times.Once);
        _repositoryMock.Verify(repo => repo.Add(user), Times.Once);
    }
}
