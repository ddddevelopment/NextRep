using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Users.Application.Services;
using Users.Domain.Exceptions;
using Users.Domain.Models;
using Users.Domain.Repositories;

namespace UsersUnitTests;

public class UsersCreateUnitTests
{
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly UsersService _service;
    private readonly IFixture _fixture;

    public UsersCreateUnitTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _service = new UsersService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Create_AddUser()
    {
        var user = _fixture.Create<User>();

        await _service.Create(user);

        _repositoryMock.Verify(repository => repository.Add(user));
    }

    [Fact]
    public async Task Create_UserAlreadyExists_ShouldThrowUserAlreadyExistsException()
    {
        var user = _fixture.Create<User>();
        _repositoryMock.SetupSequence(repository => repository.Add(user))
                        .Returns(Task.CompletedTask)
                        .Throws(new UserAlreadyExistsException(user.Email));

        await _service.Create(user);

        await Assert.ThrowsAsync<UserAlreadyExistsException>(() => _service.Create(user));
    }
}
