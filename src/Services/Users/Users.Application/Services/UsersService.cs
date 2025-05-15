using Users.Domain.Models;
using Users.Domain.Repositories;
using Users.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Users.Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;
        private readonly ILogger<UsersService>? _logger;

        public UsersService(IUsersRepository repository, ILogger<UsersService>? logger = null)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result> Create(User user)
        {
            _logger?.LogDebug("Attempting to create user: {@User}", user);

            if (user == null)
            {
                string errorMessage = "User is null";
                _logger?.LogWarning(errorMessage);
                return Result.Invalid(errorMessage);
            }

            _logger?.LogDebug("Checking if user with email {Email} exists", user.Email);
            bool isUserExists = await _repository.ExistsByEmail(user.Email);
            if (isUserExists)
            {
                _logger?.LogWarning("User with email {Email} already exists", user.Email);
                return Result.Conflict($"User with email: {user.Email} already exists");
            }

            Result result = await _repository.Add(user);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("User created successfully: {@User}", user);
            }

            return result;
        }

        public async Task<Result<User>> GetById(Guid id)
        {
            _logger?.LogDebug("Fetching user with ID: {UserId}", id);

            Result<User> result = await _repository.GetById(id);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("User retrieved successfully: {@User}", result.Value);
            }

            return result;
        }

        public async Task<Result<User>> GetByEmail(string email)
        {
            _logger?.LogDebug("Fetching user with email: {Email}", email);

            Result<User> result = await _repository.GetByEmail(email);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("User retrieved successfully: {@User}", result.Value);
            }
            
            return result;
        }

        public async Task<Result<IEnumerable<User>>> GetAll()
        {
            _logger?.LogDebug("Fetching all users");

            Result<IEnumerable<User>> result = await _repository.GetAll();

            if (result.IsSuccess) {
                _logger?.LogInformation("Successfully retrieved all users");
            }
            
            return result;
        }

        public async Task<Result<User>> Update(User user)
        {
            _logger?.LogDebug("Attempting to update user: {@User}", user);

            if (user == null)
            {
                _logger?.LogWarning("User is null");
                return Result<User>.Invalid($"User must not be null");
            }

            Result<User> result = await _repository.Update(user);

            if (result.IsSuccess) {
                _logger?.LogInformation("User updated successfully: {@User}", result.Value);
            }
            
            return result;
        }

        public async Task<Result> Delete(Guid id)
        {
            _logger?.LogDebug("Attempting to delete user with ID: {UserId}", id);

            Result result = await _repository.Remove(id);

            if (result.IsSuccess) {
                _logger?.LogInformation("User deleted successfully with ID: {UserId}", id);
            }
            
            return result;
        }
    }
}