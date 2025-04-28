using AutoMapper;
using Grpc.Core;
using Users.Domain.Models;
using Users.Domain.Services;
using Users.GrpcService;

namespace Users.GrpcService.Services;

public class UsersGrpcService : UsersGrpc.UsersGrpcBase {
    private readonly IUsersService _usersService;
    private readonly IMapper _mapper;

    public UsersGrpcService(IUsersService usersService, IMapper mapper)
    {
        _usersService = usersService;
        _mapper = mapper;
    }

    public override async Task<GetUserByEmailResponse> GetUserByEmail(GetUserByEmailRequest request, ServerCallContext context)
    {
        User user = await _usersService.GetByEmail(request.Email);
        GetUserByEmailResponse response = _mapper.Map<GetUserByEmailResponse>(user);
        return response;
    }
}
