using AutoFixture;
using Moq;
using Users.Application.Services;
using Users.Domain.Models;
using Users.Domain.Repositories;

namespace UsersUnitTests;

public class UsersServiceDeleteUnitTests 
{
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly UsersService _service;
    private readonly IFixture _fixture;

    public UsersServiceDeleteUnitTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _service = new UsersService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Delete_UserExists_ReturnSuccessResult() 
    {
        // Arrange
        Guid id = Guid.NewGuid();
        _repositoryMock.Setup(repository => repository.Remove(id))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _service.Delete(id);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(repository => repository.Remove(id), Times.Once);
    }
    
    [Fact]
    public async Task Delete_UserNotFound_ReturnNotFoundResult() 
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string expectedErrorMessage = $"User with ID: {id} not found";
        _repositoryMock.Setup(repository => repository.Remove(id))
            .ReturnsAsync(Result.NotFound(expectedErrorMessage));

        // Act
        var result = await _service.Delete(id);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        Assert.Equal(expectedErrorMessage, result.Error.Message);
        _repositoryMock.Verify(repository => repository.Remove(id), Times.Once);
    }
    
    [Fact]
    public async Task Delete_RepositoryFailure_ReturnFailureResult() 
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string expectedErrorMessage = "Database error";
        _repositoryMock.Setup(repository => repository.Remove(id))
            .ReturnsAsync(Result.Failure(expectedErrorMessage));

        // Act
        var result = await _service.Delete(id);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Unknown, result.Error.Type);
        Assert.Equal(expectedErrorMessage, result.Error.Message);
        _repositoryMock.Verify(repository => repository.Remove(id), Times.Once);
    }
}