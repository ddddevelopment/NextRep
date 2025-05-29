using AutoFixture;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ExerciseServiceUnitTests;

public class ExerciseServiceGetByIdInWorkoutUnitTests
{
    private readonly Mock<IExercisesRepository> _repositoryMock;
    private readonly Mock<IWorkoutsService> _workoutsServiceMock;
    private readonly Mock<IExerciseInfosService> _exerciseInfosServiceMock;
    private readonly Mock<ILogger<ExercisesService>> _loggerMock;
    private readonly ExercisesService _service;
    private readonly IFixture _fixture;

    public ExerciseServiceGetByIdInWorkoutUnitTests()
    {
        _repositoryMock = new Mock<IExercisesRepository>();
        _workoutsServiceMock = new Mock<IWorkoutsService>();
        _exerciseInfosServiceMock = new Mock<IExerciseInfosService>();
        _loggerMock = new Mock<ILogger<ExercisesService>>();
        _service = new ExercisesService(_repositoryMock.Object, _workoutsServiceMock.Object, _exerciseInfosServiceMock.Object, _loggerMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetByIdInWorkout_WorkoutExistsAndExerciseExists_ReturnsSuccess()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        var exercise = _fixture.Build<Exercise>().With(e => e.WorkoutId, workoutId).With(e => e.Id, exerciseId).Create();
        _workoutsServiceMock.Setup(x => x.GetById(workoutId)).ReturnsAsync(Result<Workout>.Success(_fixture.Build<Workout>().With(w => w.Id, workoutId).Create()));
        _repositoryMock.Setup(x => x.GetByIdInWorkout(workoutId, exerciseId)).ReturnsAsync(Result<Exercise>.Success(exercise));

        // Act
        var result = await _service.GetByIdInWorkout(workoutId, exerciseId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(exercise, result.Value);
        _repositoryMock.Verify(repo => repo.GetByIdInWorkout(workoutId, exerciseId), Times.Once);
    }

    [Fact]
    public async Task GetByIdInWorkout_WorkoutNotFound_ReturnsNotFound()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        _workoutsServiceMock.Setup(x => x.GetById(workoutId)).ReturnsAsync(Result<Workout>.NotFound("not found"));

        // Act
        var result = await _service.GetByIdInWorkout(workoutId, exerciseId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.GetByIdInWorkout(workoutId, exerciseId), Times.Never);
    }

    [Fact]
    public async Task GetByIdInWorkout_WorkoutExistsButExerciseNotFound_ReturnsNotFound()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        _workoutsServiceMock.Setup(x => x.GetById(workoutId)).ReturnsAsync(Result<Workout>.Success(_fixture.Build<Workout>().With(w => w.Id, workoutId).Create()));
        _repositoryMock.Setup(x => x.GetByIdInWorkout(workoutId, exerciseId)).ReturnsAsync(Result<Exercise>.NotFound("not found"));

        // Act
        var result = await _service.GetByIdInWorkout(workoutId, exerciseId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.GetByIdInWorkout(workoutId, exerciseId), Times.Once);
    }
} 