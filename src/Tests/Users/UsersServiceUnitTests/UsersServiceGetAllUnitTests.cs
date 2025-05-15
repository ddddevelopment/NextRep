using AutoFixture;
using Moq;
using Users.Application.Services;
using Users.Domain.Models;
using Users.Domain.Repositories;

namespace UsersUnitTests;

public class UsersServiceGetAllUnitTests {
    private readonly UsersService _service;
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly Fixture _fixture;

    public UsersServiceGetAllUnitTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _service = new UsersService(_repositoryMock.Object);
        _fixture = new Fixture();
    }
    
    [Fact]
    public async Task GetAll_Success_ReturnAllUsers() 
    {
        // Arrange
        IEnumerable<User> expectedUsers = _fixture.CreateMany<User>(3);
        _repositoryMock.Setup(repository => repository.GetAll())
            .ReturnsAsync(Result<IEnumerable<User>>.Success(expectedUsers));

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedUsers, result.Value);
        _repositoryMock.Verify(repository => repository.GetAll(), Times.Once);
    }
    
    [Fact]
    public async Task GetAll_EmptyList_ReturnSuccessResultWithEmptyCollection() 
    {
        // Arrange
        IEnumerable<User> expectedUsers = Enumerable.Empty<User>();
        _repositoryMock.Setup(repository => repository.GetAll())
            .ReturnsAsync(Result<IEnumerable<User>>.Success(expectedUsers));

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
        _repositoryMock.Verify(repository => repository.GetAll(), Times.Once);
    }
    
    [Fact]
    public async Task GetAll_RepositoryFailure_ReturnFailureResult() 
    {
        // Arrange
        string expectedErrorMessage = "Database error";
        _repositoryMock.Setup(repository => repository.GetAll())
            .ReturnsAsync(Result<IEnumerable<User>>.Failure(expectedErrorMessage));

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Unknown, result.Error.Type);
        Assert.Equal(expectedErrorMessage, result.Error.Message);
        _repositoryMock.Verify(repository => repository.GetAll(), Times.Once);
    }
}