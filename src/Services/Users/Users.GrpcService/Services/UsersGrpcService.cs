using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Users.Domain.Exceptions;
using Users.Domain.Models;
using Users.Domain.Services;
using Users.GrpcService;

namespace Users.GrpcService.Services;

public class UsersGrpcService : UsersGrpc.UsersGrpcBase
{
    private readonly IUsersService _usersService;
    private readonly IMapper _mapper;

    public UsersGrpcService(IUsersService usersService, IMapper mapper)
    {
        _usersService = usersService;
        _mapper = mapper;
    }

    public override async Task<GetUserResponse> GetUserByEmail(GetUserRequest request, ServerCallContext context)
    {
        try
        {
            User user = await _usersService.GetByEmail(request.Email);
            UserMessage userMessage = _mapper.Map<UserMessage>(user);
            return new GetUserResponse() { Found = true, User = userMessage };
        }
        catch (UserNotFoundException<string>) {
            return new GetUserResponse() { Found = true };
        }
    }

    public override async Task<CreateUserResponse> CreateUser(UserMessage request, ServerCallContext context)
    {
        User user = _mapper.Map<User>(request);
        await _usersService.Create(user);
        return new CreateUserResponse();
    }
}
