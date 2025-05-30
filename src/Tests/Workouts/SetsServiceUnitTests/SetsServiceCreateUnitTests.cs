using AutoFixture;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;
using Microsoft.Extensions.Logging;

namespace SetsServiceUnitTests;

public class SetsServiceCreateUnitTests
{
    private readonly Mock<ISetsRepository> _repositoryMock;
    private readonly Mock<IExercisesService> _exercisesServiceMock;
    private readonly Mock<ILogger<SetsService>> _loggerMock;
    private readonly SetsService _service;
    private readonly IFixture _fixture;

    public SetsServiceCreateUnitTests()
    {
        _repositoryMock = new Mock<ISetsRepository>();
        _exercisesServiceMock = new Mock<IExercisesService>();
        _loggerMock = new Mock<ILogger<SetsService>>();
        _service = new SetsService(_repositoryMock.Object, _exercisesServiceMock.Object, _loggerMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Create_ValidSetAndExerciseExists_ReturnsSuccess()
    {
        // Arrange
        var set = _fixture.Create<Set>();
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(set.ExerciseId, set.ExerciseId))
            .ReturnsAsync(Result<Exercise>.Success(_fixture.Build<Exercise>().With(e => e.Id, set.ExerciseId).Create()));
        _repositoryMock.Setup(x => x.Add(set)).ReturnsAsync(Result.Success());

        // Act
        var result = await _service.Create(set);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(repo => repo.Add(set), Times.Once);
    }

    [Fact]
    public async Task Create_NullSet_ReturnsInvalid()
    {
        // Act
        var result = await _service.Create(null!);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.Add(It.IsAny<Set>()), Times.Never);
    }

    [Fact]
    public async Task Create_ExerciseNotFound_ReturnsNotFound()
    {
        // Arrange
        var set = _fixture.Create<Set>();
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(set.ExerciseId, set.ExerciseId)).ReturnsAsync(Result<Exercise>.NotFound("not found"));

        // Act
        var result = await _service.Create(set);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        _repositoryMock.Verify(repo => repo.Add(set), Times.Never);
    }


    [Fact]
    public async Task Create_AddFails_ReturnsFailure()
    {
        // Arrange
        var set = _fixture.Create<Set>();
        _exercisesServiceMock.Setup(x => x.GetByIdInWorkout(set.ExerciseId, set.ExerciseId)).ReturnsAsync(Result<Exercise>.Success(_fixture.Create<Exercise>()));
        var error = Result.Failure("Some error");
        _repositoryMock.Setup(x => x.Add(set)).ReturnsAsync(error);

        // Act
        var result = await _service.Create(set);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error.Error, result.Error);
        _repositoryMock.Verify(repo => repo.Add(set), Times.Once);
    }
}