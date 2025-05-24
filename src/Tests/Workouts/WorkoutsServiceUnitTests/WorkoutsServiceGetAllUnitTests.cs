using AutoFixture;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Xunit;

namespace WorkoutsServiceUnitTests;

public class WorkoutsServiceGetAllUnitTests
{
    private readonly Mock<IWorkoutsRepository> _repositoryMock;
    private readonly WorkoutsService _service;
    private readonly IFixture _fixture;

    public WorkoutsServiceGetAllUnitTests()
    {
        _repositoryMock = new Mock<IWorkoutsRepository>();
        _service = new WorkoutsService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetAll_Success_ReturnAllWorkouts()
    {
        // Arrange
        IEnumerable<Workout> expectedWorkouts = _fixture.CreateMany<Workout>(3);
        _repositoryMock.Setup(repo => repo.GetAll()).ReturnsAsync(Result<IEnumerable<Workout>>.Success(expectedWorkouts));

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedWorkouts, result.Value);
        _repositoryMock.Verify(repo => repo.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetAll_EmptyList_ReturnSuccessResultWithEmptyCollection()
    {
        // Arrange
        IEnumerable<Workout> expectedWorkouts = Enumerable.Empty<Workout>();
        _repositoryMock.Setup(repo => repo.GetAll()).ReturnsAsync(Result<IEnumerable<Workout>>.Success(expectedWorkouts));

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetAll_Failure_ReturnFailureResult()
    {
        // Arrange
        var error = Result<IEnumerable<Workout>>.Failure("Some error");
        _repositoryMock.Setup(repo => repo.GetAll()).ReturnsAsync(error);

        // Act
        Result<IEnumerable<Workout>> result = await _service.GetAll();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error.Error, result.Error);
    }
}
