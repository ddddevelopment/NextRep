using Auth.Domain.Models;
using Auth.Domain.Services;
using Grpc.Net.Client;
using Auth.Infrastructure.Users;
using AutoMapper;
using Grpc.Core;

namespace Auth.Infrastructure.Users.Services
{
    public class UsersGrpcServiceClient : IUsersServiceClient
    {
        private readonly UsersGrpc.UsersGrpcClient _client;
        private readonly IMapper _mapper;

        public UsersGrpcServiceClient(GrpcChannel channel, IMapper mapper)
        {
            _client = new UsersGrpc.UsersGrpcClient(channel);
            _mapper = mapper;
        }

        public async Task<UserCreateResult> CreateUser(UserDto user)
        {
            CreateUserRequest request = _mapper.Map<CreateUserRequest>(user);
            CreateUserResponse response = await _client.CreateUserAsync(request);

            if (response.Success == false)
            {
                return UserCreateResult.Failure(response.ErrorMessage);
            }

            return UserCreateResult.Success();
        }

        public async Task<UserGetResult> GetUserByEmail(string email)
        {
            GetUserRequest request = new GetUserRequest() { Email = email };
            GetUserResponse response = await _client.GetUserByEmailAsync(request);

            if (response.Success == true)
            {
                if (response.Found)
                {
                    UserDto user = _mapper.Map<UserDto>(response.User);
                    return UserGetResult.Found(user);
                }

                return UserGetResult.NotFound();
            }

            return UserGetResult.Failure(response.ErrorMessage);
        }
    }
}