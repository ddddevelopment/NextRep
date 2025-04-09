using AutoFixture;
using Moq;
using Users.Application.Services;
using Users.Domain.Exceptions;
using Users.Domain.Models;
using Users.Domain.Repositories;

namespace UsersUnitTests;

public class UsersGetUnitTests {
    private readonly UsersService _service;
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly Fixture _fixture;

    public UsersGetUnitTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _service = new UsersService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Get_ReturnUser() {
        Guid id = Guid.NewGuid();
        User expectedUser = _fixture.Build<User>().With(user => user.Id, id).Create();
        _repositoryMock.Setup(repository => repository.Get(id)).Returns(Task.FromResult(expectedUser));

        var user = await _service.Get(id);

        _repositoryMock.Verify(repository => repository.Get(id), Times.Once);
        Assert.Equal(user, expectedUser);
    }

    [Fact]
    public async Task Get_NonExistentUser_ShouldThrowUserNotFoundException() {
        Guid id = Guid.NewGuid();
        User expectedUser = null;
        _repositoryMock.Setup(repository => repository.Get(id)).ReturnsAsync(expectedUser);

        await Assert.ThrowsAsync<UserNotFoundException>(() => _service.Get(id));
    }
}