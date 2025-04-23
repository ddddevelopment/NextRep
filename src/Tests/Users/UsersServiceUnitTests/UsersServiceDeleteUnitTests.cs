using AutoFixture;
using Moq;
using Users.Application.Services;
using Users.Domain.Exceptions;
using Users.Domain.Models;
using Users.Domain.Repositories;

namespace UsersUnitTests;

public class UsersServiceDeleteUnitTests {
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly UsersService _service;
    private readonly IFixture _fixture;

    public UsersServiceDeleteUnitTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _service = new UsersService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Delete_RemoveUser() {
        Guid id = Guid.NewGuid();

        await _service.Delete(id);

        _repositoryMock.Verify(repository => repository.Remove(id), Times.Once);
    }

    [Fact]
    public async Task Delete_NonexistentUser_ShouldThrowsUserNotFoundException() {
        Guid id = Guid.NewGuid();
        _repositoryMock.Setup(repository => repository.Remove(id)).ThrowsAsync(new UserNotFoundException<Guid>(id));

        await Assert.ThrowsAsync<UserNotFoundException<Guid>>(() => _service.Delete(id));
    }
}