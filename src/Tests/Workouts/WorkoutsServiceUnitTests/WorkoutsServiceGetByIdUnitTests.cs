using AutoFixture;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Xunit;

namespace WorkoutsServiceUnitTests;

public class WorkoutsServiceGetByIdUnitTests
{
    private readonly Mock<IWorkoutsRepository> _repositoryMock;
    private readonly WorkoutsService _service;
    private readonly IFixture _fixture;

    public WorkoutsServiceGetByIdUnitTests()
    {
        _repositoryMock = new Mock<IWorkoutsRepository>();
        _service = new WorkoutsService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetById_WorkoutExists_ReturnSuccessResult()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Workout expectedWorkout = _fixture.Build<Workout>().With(w => w.Id, id).Create();
        _repositoryMock.Setup(repo => repo.GetById(id)).ReturnsAsync(Result<Workout>.Success(expectedWorkout));

        // Act
        var result = await _service.GetById(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedWorkout, result.Value);
        _repositoryMock.Verify(repo => repo.GetById(id), Times.Once);
    }

    [Fact]
    public async Task GetById_WorkoutDoesNotExist_ReturnNotFoundResult()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var error = Result<Workout>.NotFound($"Workout with ID: {id} not found");
        _repositoryMock.Setup(repo => repo.GetById(id)).ReturnsAsync(error);

        // Act
        var result = await _service.GetById(id);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error.Error, result.Error);
    }
}
