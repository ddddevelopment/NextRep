using AutoFixture;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;
using Microsoft.Extensions.Logging;

namespace SetsServiceUnitTests;

public class SetsServiceGetByIdInExerciseUnitTests
{
    private readonly Mock<ISetsRepository> _repositoryMock;
    private readonly Mock<IExercisesService> _exercisesServiceMock;
    private readonly Mock<ILogger<SetsService>> _loggerMock;
    private readonly SetsService _service;
    private readonly IFixture _fixture;

    public SetsServiceGetByIdInExerciseUnitTests()
    {
        _repositoryMock = new Mock<ISetsRepository>();
        _exercisesServiceMock = new Mock<IExercisesService>();
        _loggerMock = new Mock<ILogger<SetsService>>();
        _service = new SetsService(_repositoryMock.Object, _exercisesServiceMock.Object, _loggerMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetByIdInExercise_ExerciseExistsAndSetExists_ReturnsSuccess()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();
        var setId = Guid.NewGuid();
        var set = _fixture.Build<Set>().With(s => s.ExerciseId, exerciseId).With(s => s.Id, setId).Create();
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(exerciseId, exerciseId)).ReturnsAsync(Result<Exercise>.Success(_fixture.Build<Exercise>().With(e => e.Id, exerciseId).Create()));
        _repositoryMock.Setup(x => x.GetByIdInExercise(exerciseId, setId)).ReturnsAsync(Result<Set>.Success(set));

        // Act
        var result = await _service.GetByIdInExercise(exerciseId, setId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(set, result.Value);
        _repositoryMock.Verify(repo => repo.GetByIdInExercise(exerciseId, setId), Times.Once);
    }

    [Fact]
    public async Task GetByIdInExercise_ExerciseNotFound_ReturnsNotFound()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();
        var setId = Guid.NewGuid();
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(exerciseId, exerciseId)).ReturnsAsync(Result<Exercise>.NotFound("not found"));

        // Act
        var result = await _service.GetByIdInExercise(exerciseId, setId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.GetByIdInExercise(exerciseId, setId), Times.Never);
    }

    [Fact]
    public async Task GetByIdInExercise_ExerciseExistsButSetNotFound_ReturnsNotFound()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();
        var setId = Guid.NewGuid();
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(exerciseId, exerciseId)).ReturnsAsync(Result<Exercise>.Success(_fixture.Build<Exercise>().With(e => e.Id, exerciseId).Create()));
        _repositoryMock.Setup(x => x.GetByIdInExercise(exerciseId, setId)).ReturnsAsync(Result<Set>.NotFound("not found"));

        // Act
        var result = await _service.GetByIdInExercise(exerciseId, setId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.GetByIdInExercise(exerciseId, setId), Times.Once);
    }
}
