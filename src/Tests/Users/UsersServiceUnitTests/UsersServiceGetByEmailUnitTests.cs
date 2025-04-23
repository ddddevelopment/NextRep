using System;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Users.Application.Services;
using Users.Domain.Exceptions;
using Users.Domain.Models;
using Users.Domain.Repositories;
using Xunit;

namespace UsersUnitTests;

public class UsersServiceGetByEmailUnitTests
{
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly UsersService _service;
    private readonly Fixture _fixture;

    public UsersServiceGetByEmailUnitTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _service = new UsersService(_repositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetByEmail_UserExists_ReturnUser()
    {
        User expectedUser = _fixture.Create<User>();
        string email = expectedUser.Email;
        _repositoryMock.Setup(repository => repository.GetByEmail(email)).ReturnsAsync(expectedUser);

        User result = await _service.GetByEmail(email);

        Assert.Equal(expectedUser, result);
        _repositoryMock.Verify(repository => repository.GetByEmail(email), Times.Once);
    }

    [Fact]
    public async Task GetByEmail_UserDoesNotExist_ShouldThrowUserNotFoundException()
    {
        // Arrange
        string email = _fixture.Create<string>();
        User expectedUser = null;
        _repositoryMock.Setup(repository => repository.GetByEmail(email)).ReturnsAsync(expectedUser);

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException<string>>(() => _service.GetByEmail(email));
        _repositoryMock.Verify(repository => repository.GetByEmail(email), Times.Once);
    }
} 