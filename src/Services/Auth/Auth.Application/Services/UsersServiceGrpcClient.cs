using Auth.Domain.Models;
using Auth.Domain.Services;
using Grpc.Net.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Auth.Application.Services;

namespace Auth.Application.Services;

public class UsersServiceGrpcClient : IUserServiceClient
{
    private readonly UsersGrpc
    public Task<UserDto> CreateUser(UserDto user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUser(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserDto>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> GetUser(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> GetUserByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> GetUserByUsername(string username)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> UpdateUser(UserDto user)
    {
        throw new NotImplementedException();
    }
}
