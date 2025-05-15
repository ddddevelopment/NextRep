using AutoFixture;
using Moq;
using Users.Application.Services;
using Users.Domain.Models;
using Users.Domain.Repositories;

namespace UsersUnitTests;

public class UsersUpdateUnitTests 
{
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly UsersService _service;
    private readonly IFixture _fixture;

    public UsersUpdateUnitTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _service = new UsersService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Update_ValidUser_ReturnSuccessResult() 
    {
        // Arrange
        User user = _fixture.Create<User>();
        User updatedUser = _fixture.Build<User>().With(u => u.Id, user.Id).Create();
        _repositoryMock.Setup(repository => repository.Update(user))
            .ReturnsAsync(Result<User>.Success(updatedUser));

        // Act
        var result = await _service.Update(user);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(updatedUser, result.Value);
        _repositoryMock.Verify(repository => repository.Update(user), Times.Once);
    }

    [Fact]
    public async Task Update_NullUser_ReturnInvalidResult() 
    {
        // Arrange
        User user = null;

        // Act
        var result = await _service.Update(user);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Contains("must not be null", result.Error.Message);
        _repositoryMock.Verify(repository => repository.Update(It.IsAny<User>()), Times.Never);
    }
    
    [Fact]
    public async Task Update_UserNotFound_ReturnNotFoundResult() 
    {
        // Arrange
        User user = _fixture.Create<User>();
        string expectedErrorMessage = $"User with ID: {user.Id} not found";
        _repositoryMock.Setup(repository => repository.Update(user))
            .ReturnsAsync(Result<User>.NotFound(expectedErrorMessage));

        // Act
        var result = await _service.Update(user);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        Assert.Equal(expectedErrorMessage, result.Error.Message);
        _repositoryMock.Verify(repository => repository.Update(user), Times.Once);
    }
    
    [Fact]
    public async Task Update_RepositoryFailure_ReturnFailureResult() 
    {
        // Arrange
        User user = _fixture.Create<User>();
        string expectedErrorMessage = "Database error";
        _repositoryMock.Setup(repository => repository.Update(user))
            .ReturnsAsync(Result<User>.Failure(expectedErrorMessage));

        // Act
        var result = await _service.Update(user);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Unknown, result.Error.Type);
        Assert.Equal(expectedErrorMessage, result.Error.Message);
        _repositoryMock.Verify(repository => repository.Update(user), Times.Once);
    }
}