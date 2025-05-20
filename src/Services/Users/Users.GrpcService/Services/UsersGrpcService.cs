using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Http.HttpResults;
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
        Result<User> getUserResult = await _usersService.GetByEmail(request.Email);

        if (getUserResult.IsSuccess)
        {
            UserMessage userMessage = _mapper.Map<UserMessage>(getUserResult.Value);
            return new GetUserResponse() { Success = true, Found = true, User = userMessage, ErrorMessage = string.Empty };
        }
        else if (getUserResult.Error.Type == ErrorType.NotFound)
        {
            return new GetUserResponse() { Success = true, Found = false, User = new UserMessage(), ErrorMessage = string.Empty };
        }

        return new GetUserResponse() { Success = false, Found = false, User = new UserMessage(), ErrorMessage = getUserResult.Error.Message };
    }

    public override async Task<CreateUserResponse> CreateUser(UserMessage request, ServerCallContext context)
    {
        User user = _mapper.Map<User>(request);
        Result createResult = await _usersService.Create(user);

        if (createResult.IsSuccess)
        {
            return new CreateUserResponse() { Success = true };
        }
        else
        {
            return new CreateUserResponse() { ErrorMessage = createResult.Error.Message };
        }
    }
}
