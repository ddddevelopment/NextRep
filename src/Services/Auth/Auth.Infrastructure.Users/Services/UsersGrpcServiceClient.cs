using Auth.Domain.Models;
using Auth.Domain.Services;
using Grpc.Net.Client;
using Auth.Infrastructure.Users;
using AutoMapper;

namespace Auth.Infrastructure.Users.Services
{
    public class UsersGrpcServiceClient : IUserServiceClient
    {
        private readonly UsersGrpc.UsersGrpcClient _client;
        private readonly IMapper _mapper;

        public UsersGrpcServiceClient(GrpcChannel channel, IMapper mapper)
        {
            _client = new UsersGrpc.UsersGrpcClient(channel);
            _mapper = mapper;
        }

        public async Task<UserDto> GetUserByEmail(string email)
        {
            GetUserByEmailRequest request = new GetUserByEmailRequest() { Email = email };
            GetUserByEmailResponse response = await _client.GetUserByEmailAsync(request);
            UserDto userDto = _mapper.Map<UserDto>(response);
            return userDto;
        }
    }
}