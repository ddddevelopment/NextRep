using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;

namespace SetsServiceUnitTests;

public class SetsServiceDeleteFromExerciseUnitTests
{
    private readonly Mock<ISetsRepository> _repositoryMock;
    private readonly Mock<IExercisesService> _exercisesServiceMock;
    private readonly Mock<ILogger<SetsService>> _loggerMock;
    private readonly SetsService _service;
    private readonly IFixture _fixture;

    public SetsServiceDeleteFromExerciseUnitTests()
    {
        _repositoryMock = new Mock<ISetsRepository>();
        _exercisesServiceMock = new Mock<IExercisesService>();
        _loggerMock = new Mock<ILogger<SetsService>>();
        _service = new SetsService(_repositoryMock.Object, _exercisesServiceMock.Object, _loggerMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task DeleteFromExercise_ExerciseExists_ReturnsSuccess()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        var setId = Guid.NewGuid();
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(workoutId, exerciseId)).ReturnsAsync(Result<Exercise>.Success(_fixture.Build<Exercise>().With(e => e.Id, exerciseId).Create()));
        _repositoryMock.Setup(x => x.DeleteFromExercise(exerciseId, setId)).ReturnsAsync(Result.Success());

        // Act
        var result = await _service.DeleteFromExercise(workoutId, exerciseId, setId);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(x => x.DeleteFromExercise(exerciseId, setId), Times.Once);
    }

    [Fact]
    public async Task DeleteFromExercise_ExerciseNotFound_ReturnsNotFound()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        var setId = Guid.NewGuid();
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(workoutId, exerciseId)).ReturnsAsync(Result<Exercise>.NotFound("not found"));

        // Act
        var result = await _service.DeleteFromExercise(workoutId, exerciseId, setId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        _repositoryMock.Verify(x => x.DeleteFromExercise(exerciseId, setId), Times.Never);
    }

    [Fact]
    public async Task DeleteFromExercise_RepositoryFails_ReturnsFailure()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        var setId = Guid.NewGuid();
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(workoutId, exerciseId)).ReturnsAsync(Result<Exercise>.Success(_fixture.Build<Exercise>().With(e => e.Id, exerciseId).Create()));
        var error = Result.Failure("Some error");
        _repositoryMock.Setup(x => x.DeleteFromExercise(exerciseId, setId)).ReturnsAsync(error);

        // Act
        var result = await _service.DeleteFromExercise(workoutId, exerciseId, setId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error.Error, result.Error);
        _repositoryMock.Verify(x => x.DeleteFromExercise(exerciseId, setId), Times.Once);
    }
}
