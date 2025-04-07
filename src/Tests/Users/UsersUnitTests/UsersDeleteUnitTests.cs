using AutoFixture;
using Moq;
using Users.Application.Services;
using Users.Domain.Models;
using Users.Domain.Repositories;

namespace UsersUnitTests;

public class UsersDeleteUnitTests {
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly UsersService _service;
    private readonly IFixture _fixture;

    public UsersDeleteUnitTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _service = new UsersService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Delete_ShouldRemoveUser() {
        Guid id = Guid.NewGuid();

        await _service.Delete(id);

        _repositoryMock.Verify(repository => repository.Remove(id), Times.Once);
    }
}