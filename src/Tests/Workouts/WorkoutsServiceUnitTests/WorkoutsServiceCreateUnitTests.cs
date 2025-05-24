using AutoFixture;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;

namespace WorkoutsServiceUnitTests;

public class WorkoutsServiceCreateUnitTests
{
    private readonly Mock<IWorkoutsRepository> _repositoryMock;
    private readonly WorkoutsService _service;
    private readonly IFixture _fixture;

    public WorkoutsServiceCreateUnitTests()
    {
        _repositoryMock = new Mock<IWorkoutsRepository>();
        _service = new WorkoutsService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Create_ValidWorkout_ReturnSuccessResult()
    {
        // Arrange
        var workout = _fixture.Create<Workout>();
        _repositoryMock.Setup(repo => repo.Add(workout)).ReturnsAsync(Result.Success());

        // Act
        var result = await _service.Create(workout);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(repo => repo.Add(workout), Times.Once);
    }

    [Fact]
    public async Task Create_NullWorkout_ReturnInvalidResult()
    {
        // Arrange
        Workout workout = null;

        // Act
        var result = await _service.Create(workout);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task Create_AddFails_ReturnFailureResult()
    {
        // Arrange
        var workout = _fixture.Create<Workout>();
        var error = Result.Invalid("Some error");
        _repositoryMock.Setup(repo => repo.Add(workout)).ReturnsAsync(error);

        // Act
        var result = await _service.Create(workout);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error.Error, result.Error);
    }
}
