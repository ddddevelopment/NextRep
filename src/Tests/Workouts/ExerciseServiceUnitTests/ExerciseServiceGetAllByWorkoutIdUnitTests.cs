using AutoFixture;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ExerciseServiceUnitTests;

public class ExerciseServiceGetAllByWorkoutIdUnitTests
{
    private readonly Mock<IExercisesRepository> _repositoryMock;
    private readonly Mock<IWorkoutsService> _workoutsServiceMock;
    private readonly Mock<IExerciseInfosService> _exerciseInfosServiceMock;
    private readonly Mock<ILogger<ExercisesService>> _loggerMock;
    private readonly ExercisesService _service;
    private readonly IFixture _fixture;

    public ExerciseServiceGetAllByWorkoutIdUnitTests()
    {
        _repositoryMock = new Mock<IExercisesRepository>();
        _workoutsServiceMock = new Mock<IWorkoutsService>();
        _exerciseInfosServiceMock = new Mock<IExerciseInfosService>();
        _loggerMock = new Mock<ILogger<ExercisesService>>();
        _service = new ExercisesService(_repositoryMock.Object, _workoutsServiceMock.Object, _exerciseInfosServiceMock.Object, _loggerMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetAllByWorkoutId_WorkoutExists_ReturnsExercises()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exercises = _fixture.Build<Exercise>().With(e => e.WorkoutId, workoutId).CreateMany(3);
        _workoutsServiceMock.Setup(x => x.GetById(workoutId)).ReturnsAsync(Result<Workout>.Success(_fixture.Build<Workout>().With(w => w.Id, workoutId).Create()));
        _repositoryMock.Setup(x => x.GetAllByWorkoutId(workoutId)).ReturnsAsync(Result<IEnumerable<Exercise>>.Success(exercises));

        // Act
        var result = await _service.GetAllByWorkoutId(workoutId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(exercises, result.Value);
        _repositoryMock.Verify(repo => repo.GetAllByWorkoutId(workoutId), Times.Once);
    }

    [Fact]
    public async Task GetAllByWorkoutId_WorkoutNotFound_ReturnsNotFound()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        _workoutsServiceMock.Setup(x => x.GetById(workoutId)).ReturnsAsync(Result<Workout>.NotFound("not found"));

        // Act
        var result = await _service.GetAllByWorkoutId(workoutId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.GetAllByWorkoutId(workoutId), Times.Never);
    }

    [Fact]
    public async Task GetAllByWorkoutId_RepositoryFails_ReturnsFailure()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        _workoutsServiceMock.Setup(x => x.GetById(workoutId)).ReturnsAsync(Result<Workout>.Success(_fixture.Build<Workout>().With(w => w.Id, workoutId).Create()));
        var error = Result<IEnumerable<Exercise>>.Failure("Some error");
        _repositoryMock.Setup(x => x.GetAllByWorkoutId(workoutId)).ReturnsAsync(error);

        // Act
        var result = await _service.GetAllByWorkoutId(workoutId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error.Error, result.Error);
        _repositoryMock.Verify(repo => repo.GetAllByWorkoutId(workoutId), Times.Once);
    }
} 