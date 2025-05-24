using AutoFixture;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Xunit;

namespace WorkoutsServiceUnitTests;

public class WorkoutsServiceUpdateUnitTests
{
    private readonly Mock<IWorkoutsRepository> _repositoryMock;
    private readonly WorkoutsService _service;
    private readonly IFixture _fixture;

    public WorkoutsServiceUpdateUnitTests()
    {
        _repositoryMock = new Mock<IWorkoutsRepository>();
        _service = new WorkoutsService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Update_ValidWorkout_ReturnSuccessResult()
    {
        // Arrange
        Workout workout = _fixture.Create<Workout>();
        Workout updatedWorkout = _fixture.Build<Workout>().With(w => w.Id, workout.Id).Create();
        _repositoryMock.Setup(repo => repo.Update(workout)).ReturnsAsync(Result<Workout>.Success(updatedWorkout));

        // Act
        var result = await _service.Update(workout);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(updatedWorkout, result.Value);
        _repositoryMock.Verify(repo => repo.Update(workout), Times.Once);
    }

    [Fact]
    public async Task Update_NullWorkout_ReturnInvalidResult()
    {
        // Arrange
        Workout workout = null;

        // Act
        var result = await _service.Update(workout);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task Update_UpdateFails_ReturnFailureResult()
    {
        // Arrange
        Workout workout = _fixture.Create<Workout>();
        var error = Result<Workout>.Failure("Some error");
        _repositoryMock.Setup(repo => repo.Update(workout)).ReturnsAsync(error);

        // Act
        var result = await _service.Update(workout);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error.Error, result.Error);
    }
}
