using AutoFixture;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;
using Microsoft.Extensions.Logging;

namespace ExerciseServiceUnitTests;

public class ExerciseServiceUpdateUnitTests
{
    private readonly Mock<IExercisesRepository> _repositoryMock;
    private readonly Mock<IWorkoutsService> _workoutsServiceMock;
    private readonly Mock<IExerciseInfosService> _exerciseInfosServiceMock;
    private readonly Mock<ILogger<ExercisesService>> _loggerMock;
    private readonly ExercisesService _service;
    private readonly IFixture _fixture;

    public ExerciseServiceUpdateUnitTests()
    {
        _repositoryMock = new Mock<IExercisesRepository>();
        _workoutsServiceMock = new Mock<IWorkoutsService>();
        _exerciseInfosServiceMock = new Mock<IExerciseInfosService>();
        _loggerMock = new Mock<ILogger<ExercisesService>>();
        _service = new ExercisesService(_repositoryMock.Object, _workoutsServiceMock.Object, _exerciseInfosServiceMock.Object, _loggerMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Update_ValidExercise_ReturnsSuccess()
    {
        // Arrange
        var exercise = _fixture.Create<Exercise>();
        var updatedExercise = new Exercise() { Id = exercise.Id, ExerciseInfoId = exercise.ExerciseInfoId, Notes = exercise.Notes, Sets = exercise.Sets, WorkoutId = exercise.WorkoutId };
        _workoutsServiceMock.Setup(x => x.GetById(exercise.WorkoutId)).ReturnsAsync(Result<Workout>.Success(_fixture.Build<Workout>().With(w => w.Id, exercise.WorkoutId).Create()));
        _repositoryMock.Setup(x => x.Update(exercise)).ReturnsAsync(Result<Exercise>.Success(updatedExercise));

        // Act
        var result = await _service.Update(exercise);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(updatedExercise, result.Value);
        _repositoryMock.Verify(x => x.Update(exercise), Times.Once);
    }

    [Fact]
    public async Task Update_NullExercise_ReturnsInvalid()
    {
        // Act
        var result = await _service.Update(null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.Update(It.IsAny<Exercise>()), Times.Never);
    }

    [Fact]
    public async Task Update_WorkoutNotFound_ReturnsNotFound()
    {
        // Arrange
        var exercise = _fixture.Create<Exercise>();
        _workoutsServiceMock.Setup(x => x.GetById(exercise.WorkoutId)).ReturnsAsync(Result<Workout>.NotFound("not found"));

        // Act
        var result = await _service.Update(exercise);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.Update(exercise), Times.Never);
    }

    [Fact]
    public async Task Update_RepositoryFails_ReturnsFailure()
    {
        // Arrange
        var exercise = _fixture.Create<Exercise>();
        _workoutsServiceMock.Setup(x => x.GetById(exercise.WorkoutId)).ReturnsAsync(Result<Workout>.Success(_fixture.Build<Workout>().With(w => w.Id, exercise.WorkoutId).Create()));
        var error = Result<Exercise>.Failure("Some error");
        _repositoryMock.Setup(x => x.Update(exercise)).ReturnsAsync(error);

        // Act
        var result = await _service.Update(exercise);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error.Error, result.Error);
        _repositoryMock.Verify(repo => repo.Update(exercise), Times.Once);
    }
} 