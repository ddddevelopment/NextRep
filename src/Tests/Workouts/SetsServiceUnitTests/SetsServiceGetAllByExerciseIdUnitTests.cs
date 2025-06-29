using AutoFixture;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;
using Microsoft.Extensions.Logging;

namespace SetsServiceUnitTests;

public class SetsServiceGetAllByExerciseIdUnitTests
{
    private readonly Mock<ISetsRepository> _repositoryMock;
    private readonly Mock<IExercisesService> _exercisesServiceMock;
    private readonly Mock<ILogger<SetsService>> _loggerMock;
    private readonly SetsService _service;
    private readonly IFixture _fixture;

    public SetsServiceGetAllByExerciseIdUnitTests()
    {
        _repositoryMock = new Mock<ISetsRepository>();
        _exercisesServiceMock = new Mock<IExercisesService>();
        _loggerMock = new Mock<ILogger<SetsService>>();
        _service = new SetsService(_repositoryMock.Object, _exercisesServiceMock.Object, _loggerMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetAllByExerciseId_ExerciseExists_ReturnsSets()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        var sets = _fixture.Build<Set>().With(s => s.ExerciseId, exerciseId).CreateMany(3);
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(workoutId, exerciseId)).ReturnsAsync(Result<Exercise>.Success(_fixture.Build<Exercise>().With(e => e.Id, exerciseId).Create()));
        _repositoryMock.Setup(x => x.GetAllByExerciseId(exerciseId)).ReturnsAsync(Result<IEnumerable<Set>>.Success(sets));

        // Act
        var result = await _service.GetAllByExerciseId(workoutId, exerciseId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(sets, result.Value);
        _repositoryMock.Verify(repo => repo.GetAllByExerciseId(exerciseId), Times.Once);
    }

    [Fact]
    public async Task GetAllByExerciseId_ExerciseNotFound_ReturnsNotFound()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(workoutId, exerciseId)).ReturnsAsync(Result<Exercise>.NotFound("not found"));

        // Act
        var result = await _service.GetAllByExerciseId(workoutId, exerciseId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.GetAllByExerciseId(exerciseId), Times.Never);
    }

    [Fact]
    public async Task GetAllByExerciseId_RepositoryFails_ReturnsFailure()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(workoutId, exerciseId)).ReturnsAsync(Result<Exercise>.Success(_fixture.Build<Exercise>().With(e => e.Id, exerciseId).Create()));
        var error = Result<IEnumerable<Set>>.Failure("Some error");
        _repositoryMock.Setup(x => x.GetAllByExerciseId(exerciseId)).ReturnsAsync(error);

        // Act
        var result = await _service.GetAllByExerciseId(workoutId, exerciseId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error.Error, result.Error);
        _repositoryMock.Verify(repo => repo.GetAllByExerciseId(exerciseId), Times.Once);
    }
}
