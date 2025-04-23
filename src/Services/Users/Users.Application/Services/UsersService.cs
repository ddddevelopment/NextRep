using Users.Domain.Exceptions;
using Users.Domain.Models;
using Users.Domain.Repositories;
using Users.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Users.Application.Services {
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;
        private readonly ILogger<UsersService>? _logger;

        public UsersService(IUsersRepository repository, ILogger<UsersService>? logger = null)
        {   
            _repository = repository;    
            _logger = logger;
        }

        public async Task Create(User user)
        {
            _logger?.LogDebug("Attempting to create user: {@User}", user);

            if (user == null) {
                _logger?.LogWarning("User is null");
                throw new ArgumentNullException("User must be not null");
            }

            _logger?.LogDebug("Checking if user with email {Email} exists", user.Email);
            bool isUserExists = await _repository.ExistsByEmail(user.Email);
            if (isUserExists) {
                _logger?.LogWarning("User with email {Email} already exists", user.Email);
                throw new UserAlreadyExistsException(user.Email);
            }
            
            await _repository.Add(user);

            _logger?.LogInformation("User created successfully: {@User}", user);
        }

        public async Task<User> GetById(Guid id)
        {
            _logger?.LogDebug("Fetching user with ID: {UserId}", id);

            User user = await _repository.GetById(id);

            if (user == null) {
                _logger?.LogWarning("User with ID {UserId} not found", id);
                throw new UserNotFoundException<Guid>(id);
            }

            _logger?.LogInformation("User retrieved successfully: {@User}", user);
            return user;
        }

        public async Task<User> GetByEmail(string email)
        {
            _logger?.LogDebug("Fetching user with email: {Email}", email);
            
            User user = await _repository.GetByEmail(email);

            if (user == null) {
                _logger?.LogWarning("User with email: {Email} not found", email);
                throw new UserNotFoundException<string>(email);
            }

            _logger?.LogInformation("User retrieved successfully: {@User}", user);
            return user;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            _logger?.LogDebug("Fetching all users");

            IEnumerable<User> users = await _repository.GetAll();
            _logger?.LogInformation("Successfully retrieved all users");
            return users;
        }

        public async Task<User> Update(User user)
        {
            _logger?.LogDebug("Attempting to update user: {@User}", user);

            if (user == null) {
                _logger?.LogWarning("User is null");
                throw new ArgumentNullException("User must be not null");
            }

            User updatedUser = await _repository.Update(user);
            _logger?.LogInformation("User updated successfully: {@User}", updatedUser);
            return updatedUser;
        }
        
        public async Task Delete(Guid id)
        {
            _logger?.LogDebug("Attempting to delete user with ID: {UserId}", id);
            await _repository.Remove(id);
            _logger?.LogInformation("User deleted successfully with ID: {UserId}", id);
        }

    }
}