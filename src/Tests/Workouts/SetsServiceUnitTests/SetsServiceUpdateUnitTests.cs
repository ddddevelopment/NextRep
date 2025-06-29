using AutoFixture;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;
using Microsoft.Extensions.Logging;

namespace SetsServiceUnitTests;

public class SetsServiceUpdateUnitTests
{
    private readonly Mock<ISetsRepository> _repositoryMock;
    private readonly Mock<IExercisesService> _exercisesServiceMock;
    private readonly Mock<ILogger<SetsService>> _loggerMock;
    private readonly SetsService _service;
    private readonly IFixture _fixture;

    public SetsServiceUpdateUnitTests()
    {
        _repositoryMock = new Mock<ISetsRepository>();
        _exercisesServiceMock = new Mock<IExercisesService>();
        _loggerMock = new Mock<ILogger<SetsService>>();
        _service = new SetsService(_repositoryMock.Object, _exercisesServiceMock.Object, _loggerMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Update_ValidSet_ReturnsSuccess()
    {
        // Arrange
        var set = _fixture.Create<Set>();
        var workoutId = Guid.NewGuid();
        var updatedSet = new Set() { Id = set.Id, ExerciseId = set.ExerciseId, Notes = set.Notes, Reps = set.Reps, Weight = set.Weight };
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(workoutId, set.ExerciseId)).ReturnsAsync(Result<Exercise>.Success(_fixture.Build<Exercise>().With(e => e.Id, set.ExerciseId).Create()));
        _repositoryMock.Setup(x => x.Update(set)).ReturnsAsync(Result<Set>.Success(updatedSet));

        // Act
        var result = await _service.Update(workoutId, set);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(updatedSet, result.Value);
        _repositoryMock.Verify(x => x.Update(set), Times.Once);
    }

    [Fact]
    public async Task Update_NullSet_ReturnsInvalid()
    {
        // Act
        var workoutId = Guid.NewGuid();
        var result = await _service.Update(workoutId, null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.Update(It.IsAny<Set>()), Times.Never);
    }

    [Fact]
    public async Task Update_ExerciseNotFound_ReturnsNotFound()
    {
        // Arrange
        var set = _fixture.Create<Set>();
        var workoutId = Guid.NewGuid();
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(workoutId, set.ExerciseId)).ReturnsAsync(Result<Exercise>.NotFound("not found"));

        // Act
        var result = await _service.Update(workoutId, set);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.Update(set), Times.Never);
    }

    [Fact]
    public async Task Update_RepositoryFails_ReturnsFailure()
    {
        // Arrange
        var set = _fixture.Create<Set>();
        var workoutId = Guid.NewGuid();
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(workoutId, set.ExerciseId)).ReturnsAsync(Result<Exercise>.Success(_fixture.Build<Exercise>().With(e => e.Id, set.ExerciseId).Create()));
        var error = Result<Set>.Failure("Some error");
        _repositoryMock.Setup(x => x.Update(set)).ReturnsAsync(error);

        // Act
        var result = await _service.Update(workoutId, set);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error.Error, result.Error);
        _repositoryMock.Verify(repo => repo.Update(set), Times.Once);
    }
}
