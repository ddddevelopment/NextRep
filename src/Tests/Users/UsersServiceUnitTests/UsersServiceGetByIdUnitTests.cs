using AutoFixture;
using Moq;
using Users.Application.Services;
using Users.Domain.Models;
using Users.Domain.Repositories;

namespace UsersUnitTests;

public class UsersServiceGetByIdUnitTests 
{
    private readonly UsersService _service;
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly Fixture _fixture;

    public UsersServiceGetByIdUnitTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _service = new UsersService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetById_UserExists_ReturnSuccessResult() 
    {
        // Arrange
        Guid id = Guid.NewGuid();
        User expectedUser = _fixture.Build<User>().With(user => user.Id, id).Create();
        _repositoryMock.Setup(repository => repository.GetById(id))
            .ReturnsAsync(Result<User>.Success(expectedUser));

        // Act
        var result = await _service.GetById(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedUser, result.Value);
        _repositoryMock.Verify(repository => repository.GetById(id), Times.Once);
    }

    [Fact]
    public async Task GetById_UserDoesNotExist_ReturnNotFoundResult() 
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string expectedErrorMessage = $"User with ID: {id} not found";
        _repositoryMock.Setup(repository => repository.GetById(id))
            .ReturnsAsync(Result<User>.NotFound(expectedErrorMessage));

        // Act
        var result = await _service.GetById(id);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        Assert.Equal(expectedErrorMessage, result.Error.Message);
        _repositoryMock.Verify(repository => repository.GetById(id), Times.Once);
    }

    [Fact]
    public async Task GetById_RepositoryFailure_ReturnFailureResult() 
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string expectedErrorMessage = "Database error";
        _repositoryMock.Setup(repository => repository.GetById(id))
            .ReturnsAsync(Result<User>.Failure(expectedErrorMessage));

        // Act
        var result = await _service.GetById(id);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Unknown, result.Error.Type);
        Assert.Equal(expectedErrorMessage, result.Error.Message);
        _repositoryMock.Verify(repository => repository.GetById(id), Times.Once);
    }
}