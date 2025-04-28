using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Users.Application.Services;
using Users.Domain.Exceptions;
using Users.Domain.Models;
using Users.Domain.Repositories;

namespace UsersUnitTests;

public class UsersServiceCreateUnitTests
{
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly UsersService _service;
    private readonly IFixture _fixture;

    public UsersServiceCreateUnitTests()
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
        _repositoryMock.Setup(repository => repository.ExistsByEmail(user.Email)).ReturnsAsync(true);

        await Assert.ThrowsAsync<UserAlreadyExistsException>(() => _service.Create(user));
    }

    [Fact]
    public async Task Create_NullUser_ShouldThrowsArgumentNullException() {
        User user = null;

        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.Create(user));
    }
}
