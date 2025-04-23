using AutoFixture;
using Moq;
using Users.Application.Services;
using Users.Domain.Exceptions;
using Users.Domain.Models;
using Users.Domain.Repositories;

namespace UsersUnitTests;

public class UsersUpdateUnitTests {
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly UsersService _service;
    private readonly IFixture _fixture;

    public UsersUpdateUnitTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _service = new UsersService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Update_ShouldUpdateAndReturnUser() {
        User oldUser = _fixture.Create<User>();
        User expectedNewUser = _fixture.Build<User>().With(user => user.Id, oldUser.Id).Create();
        _repositoryMock.Setup(repository => repository.Update(oldUser)).ReturnsAsync(expectedNewUser);

        User newUser = await _service.Update(oldUser);

        _repositoryMock.Verify(repository => repository.Update(oldUser), Times.Once);
        Assert.Equal(oldUser.Id, newUser.Id);
        Assert.Equal(newUser, expectedNewUser);
    }

    [Fact]
    public async Task Update_NonExistentUser_ShouldThrowUserNotFoundException() {
        User user = _fixture.Create<User>();
        _repositoryMock.Setup(repository => repository.Update(user)).ThrowsAsync(new UserNotFoundException<Guid>(user.Id));

        await Assert.ThrowsAsync<UserNotFoundException<Guid>>(() => _service.Update(user));
    }

    [Fact]
    public async Task Update_NullUser_ShouldThrowsArgumentNullException() {
        User user = null;

        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.Update(user));
    }
}