using Grpc.Core;
using Users.Domain.Services;
using Users.GrpcService;

namespace Users.GrpcService.Services;

public class UsersGrpcService : UsersGrpc.UsersGrpcBase
{
    private readonly IUsersService _usersService;

    public UsersGrpcService(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public override async Task<UserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        var user = await _usersService.Get(Guid.Parse(request.Id));

        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"User with Id = {request.Id} not found"));
        }

        return new UserResponse
        {
            Id = user.Id.ToString(),
            Name = user.Name,
            Email = user.Email,
            Telephone = user.Telephone
        };
    }

    public override async Task<UsersResponse> GetAllUsers(GetAllUsersRequest request, ServerCallContext context)
    {
        var users = await _usersService.GetAll();

        var response = new UsersResponse();

        response.Users.AddRange(users.Select(user => new UserResponse()
        {
            Id = user.Id.ToString(),
            Name = user.Name,
            Email = user.Email,
            Telephone = user.Telephone
        }));
        
        return response;
    }
 
}