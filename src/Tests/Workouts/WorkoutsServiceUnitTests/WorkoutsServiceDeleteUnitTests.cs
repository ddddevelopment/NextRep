using AutoFixture;
using Moq;
using Workouts.Application.Services;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Xunit;

namespace WorkoutsServiceUnitTests;

public class WorkoutsServiceDeleteUnitTests
{
    private readonly Mock<IWorkoutsRepository> _repositoryMock;
    private readonly WorkoutsService _service;
    private readonly IFixture _fixture;

    public WorkoutsServiceDeleteUnitTests()
    {
        _repositoryMock = new Mock<IWorkoutsRepository>();
        _service = new WorkoutsService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Delete_WorkoutExists_ReturnSuccessResult()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        _repositoryMock.Setup(repo => repo.Delete(id)).ReturnsAsync(Result.Success());

        // Act
        var result = await _service.Delete(id);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(repo => repo.Delete(id), Times.Once);
    }

    [Fact]
    public async Task Delete_WorkoutNotFound_ReturnNotFoundResult()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var error = Result.NotFound($"Workout with ID: {id} not found");
        _repositoryMock.Setup(repo => repo.Delete(id)).ReturnsAsync(error);

        // Act
        var result = await _service.Delete(id);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error.Error, result.Error);
    }
}
