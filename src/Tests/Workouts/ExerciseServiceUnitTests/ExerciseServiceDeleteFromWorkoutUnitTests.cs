using AutoFixture;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ExerciseServiceUnitTests;

public class ExerciseServiceDeleteFromWorkoutUnitTests
{
    private readonly Mock<IExercisesRepository> _repositoryMock;
    private readonly Mock<IWorkoutsService> _workoutsServiceMock;
    private readonly Mock<IExerciseInfosService> _exerciseInfosServiceMock;
    private readonly Mock<ILogger<ExercisesService>> _loggerMock;
    private readonly ExercisesService _service;
    private readonly IFixture _fixture;

    public ExerciseServiceDeleteFromWorkoutUnitTests()
    {
        _repositoryMock = new Mock<IExercisesRepository>();
        _workoutsServiceMock = new Mock<IWorkoutsService>();
        _exerciseInfosServiceMock = new Mock<IExerciseInfosService>();
        _loggerMock = new Mock<ILogger<ExercisesService>>();
        _service = new ExercisesService(_repositoryMock.Object, _workoutsServiceMock.Object, _exerciseInfosServiceMock.Object, _loggerMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task DeleteFromWorkout_WorkoutExists_ReturnsSuccess()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        _workoutsServiceMock.Setup(x => x.GetById(workoutId)).ReturnsAsync(Result<Workout>.Success(_fixture.Build<Workout>().With(w => w.Id, workoutId).Create()));
        _repositoryMock.Setup(x => x.DeleteFromWorkout(workoutId, exerciseId)).ReturnsAsync(Result.Success());

        // Act
        var result = await _service.DeleteFromWorkout(workoutId, exerciseId);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(x => x.DeleteFromWorkout(workoutId, exerciseId), Times.Once);
    }

    [Fact]
    public async Task DeleteFromWorkout_WorkoutNotFound_ReturnsNotFound()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        _workoutsServiceMock.Setup(x => x.GetById(workoutId)).ReturnsAsync(Result<Workout>.NotFound("not found"));

        // Act
        var result = await _service.DeleteFromWorkout(workoutId, exerciseId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        _repositoryMock.Verify(x => x.DeleteFromWorkout(workoutId, exerciseId), Times.Never);
    }

    [Fact]
    public async Task DeleteFromWorkout_RepositoryFails_ReturnsFailure()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        _workoutsServiceMock.Setup(x => x.GetById(workoutId)).ReturnsAsync(Result<Workout>.Success(_fixture.Build<Workout>().With(w => w.Id, workoutId).Create()));
        var error = Result.Failure("Some error");
        _repositoryMock.Setup(x => x.DeleteFromWorkout(workoutId, exerciseId)).ReturnsAsync(error);

        // Act
        var result = await _service.DeleteFromWorkout(workoutId, exerciseId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error.Error, result.Error);
        _repositoryMock.Verify(x => x.DeleteFromWorkout(workoutId, exerciseId), Times.Once);
    }
} 