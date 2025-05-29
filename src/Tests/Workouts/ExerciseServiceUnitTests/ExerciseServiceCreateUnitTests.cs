using AutoFixture;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ExerciseServiceUnitTests;

public class ExerciseServiceCreateUnitTests
{
    private readonly Mock<IExercisesRepository> _repositoryMock;
    private readonly Mock<IWorkoutsService> _workoutsServiceMock;
    private readonly Mock<IExerciseInfosService> _exerciseInfosServiceMock;
    private readonly Mock<ILogger<ExercisesService>> _loggerMock;
    private readonly ExercisesService _service;
    private readonly IFixture _fixture;

    public ExerciseServiceCreateUnitTests()
    {
        _repositoryMock = new Mock<IExercisesRepository>();
        _workoutsServiceMock = new Mock<IWorkoutsService>();
        _exerciseInfosServiceMock = new Mock<IExerciseInfosService>();
        _loggerMock = new Mock<ILogger<ExercisesService>>();
        _service = new ExercisesService(_repositoryMock.Object, _workoutsServiceMock.Object, _exerciseInfosServiceMock.Object, _loggerMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Create_ValidExercise_ReturnsSuccess()
    {
        // Arrange
        var exercise = _fixture.Create<Exercise>();
        _workoutsServiceMock.Setup(x => x.GetById(exercise.WorkoutId)).ReturnsAsync(Result<Workout>.Success(_fixture.Build<Workout>().With(w => w.Id, exercise.WorkoutId).Create()));
        _exerciseInfosServiceMock.Setup(x => x.GetById(exercise.ExerciseInfoId)).ReturnsAsync(Result<ExerciseInfo>.Success(_fixture.Build<ExerciseInfo>().With(ei => ei.Id, exercise.ExerciseInfoId).Create()));
        _repositoryMock.Setup(x => x.GetByIdInWorkout(exercise.WorkoutId, exercise.Id)).ReturnsAsync(Result<Exercise>.NotFound("not found"));
        _repositoryMock.Setup(x => x.Add(exercise)).ReturnsAsync(Result.Success());

        // Act
        var result = await _service.Create(exercise);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(x => x.Add(exercise), Times.Once);
    }

    [Fact]
    public async Task Create_NullExercise_ReturnsInvalid()
    {
        // Act
        var result = await _service.Create(null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.Add(It.IsAny<Exercise>()), Times.Never);
    }

    [Fact]
    public async Task Create_WorkoutNotFound_ReturnsNotFound()
    {
        // Arrange
        var exercise = _fixture.Create<Exercise>();
        _workoutsServiceMock.Setup(x => x.GetById(exercise.WorkoutId)).ReturnsAsync(Result<Workout>.NotFound("not found"));

        // Act
        var result = await _service.Create(exercise);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.Add(exercise), Times.Never);
    }

    [Fact]
    public async Task Create_ExerciseInfoNotFound_ReturnsNotFound()
    {
        // Arrange
        var exercise = _fixture.Create<Exercise>();
        _workoutsServiceMock.Setup(x => x.GetById(exercise.WorkoutId)).ReturnsAsync(Result<Workout>.Success(_fixture.Create<Workout>()));
        _exerciseInfosServiceMock.Setup(x => x.GetById(exercise.ExerciseInfoId)).ReturnsAsync(Result<ExerciseInfo>.NotFound("not found"));

        // Act
        var result = await _service.Create(exercise);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.Add(exercise), Times.Never);
    }

    [Fact]
    public async Task Create_ExerciseAlreadyExists_ReturnsConflict()
    {
        // Arrange
        var exercise = _fixture.Create<Exercise>();
        _workoutsServiceMock.Setup(x => x.GetById(exercise.WorkoutId)).ReturnsAsync(Result<Workout>.Success(_fixture.Build<Workout>().With(w => w.Id, exercise.WorkoutId).Create()));
        _exerciseInfosServiceMock.Setup(x => x.GetById(exercise.ExerciseInfoId)).ReturnsAsync(Result<ExerciseInfo>.Success(_fixture.Build<ExerciseInfo>().With(ei => ei.Id, exercise.ExerciseInfoId).Create()));
        _repositoryMock.Setup(x => x.GetByIdInWorkout(exercise.WorkoutId, exercise.Id)).ReturnsAsync(Result<Exercise>.Success(_fixture.Create<Exercise>()));

        // Act
        var result = await _service.Create(exercise);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.Add(exercise), Times.Never);
    }

    [Fact]
    public async Task Create_AddFails_ReturnsFailure()
    {
        // Arrange
        var exercise = _fixture.Create<Exercise>();
        _workoutsServiceMock.Setup(x => x.GetById(exercise.WorkoutId)).ReturnsAsync(Result<Workout>.Success(_fixture.Create<Workout>()));
        _exerciseInfosServiceMock.Setup(x => x.GetById(exercise.ExerciseInfoId)).ReturnsAsync(Result<ExerciseInfo>.Success(_fixture.Create<ExerciseInfo>()));
        _repositoryMock.Setup(x => x.GetByIdInWorkout(exercise.WorkoutId, exercise.Id)).ReturnsAsync(Result<Exercise>.NotFound("not found"));
        var error = Result.Failure("Some error");
        _repositoryMock.Setup(x => x.Add(exercise)).ReturnsAsync(error);

        // Act
        var result = await _service.Create(exercise);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error.Error, result.Error);
        _repositoryMock.Verify(repo => repo.Add(exercise), Times.Once);
    }
} 