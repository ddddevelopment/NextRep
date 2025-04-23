using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Users.DAL.Entities;
using Users.Domain.Exceptions;
using Users.Domain.Models;
using Users.Domain.Repositories;

namespace Users.DAL.Repositories {
    public class UsersEFRepository : IUsersRepository {
        private readonly UsersDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersEFRepository>? _logger;

        public UsersEFRepository(UsersDbContext context, IMapper mapper, ILogger<UsersEFRepository>? logger = null)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Add(User user)
        {
            _logger?.LogDebug("Adding user to database: {@User}", user);

            UserEntity userEntity = _mapper.Map<UserEntity>(user);
            _context.Users.Add(userEntity);
            await _context.SaveChangesAsync();

            _logger?.LogDebug("User added to database successfully: {@User}", user);
        }

        public async Task<User> GetById(Guid id)
        {
            _logger?.LogDebug("Fetching from database user with ID: {UserId}", id);

            UserEntity? foundUser = await _context.Users.FindAsync(id);
            if (foundUser == null) {
                _logger?.LogWarning("User with ID {UserId} not found in database", id);
            }
            else {
                _logger?.LogDebug("Successfully fetched user from database: {@User}", foundUser);
            }

            User? user = _mapper.Map<User?>(foundUser);
            return user;
        }

        public async Task<User> GetByEmail(string email)
        {
            _logger?.LogDebug("Fetching from database user with email: {Email}", email);

            UserEntity? foundUser = await _context.Users.FirstOrDefaultAsync(user => user.email == email);
            if (foundUser == null) {
                _logger?.LogWarning("User with email: {Email} not found in database", email);
            }
            else {
                _logger?.LogDebug("Successfully fetched user from database: {@User}", foundUser);
            }

            User? user = _mapper.Map<User?>(foundUser);
            return user;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            _logger?.LogDebug("Fetching all users from database");

            IEnumerable<UserEntity> foundUsers = _context.Users.AsEnumerable();
            IEnumerable<User> users = _mapper.Map<IEnumerable<User>>(foundUsers);

            _logger?.LogDebug("Successfully fetched all users from database");
            return users;
        }

        public async Task<User> Update(User user)
        {
            _logger?.LogDebug("Updating user in database: {@User}", user);

            UserEntity? foundUser = await _context.Users.FindAsync(user.Id);
            if (foundUser == null) {
                _logger?.LogWarning("User with ID {UserId} not found for update in database", user.Id);
                throw new UserNotFoundException<Guid>(user.Id);
            }
            _mapper.Map(user, foundUser);
            
            _context.Users.Update(foundUser);
            await _context.SaveChangesAsync();

            User updatedUser = _mapper.Map<User>(foundUser);
            _logger?.LogDebug("User updated successfully in database: {@User}", updatedUser);
            return updatedUser;
        }
        
        public async Task Remove(Guid id)
        {
            _logger?.LogDebug("Removing user with ID: {UserId} from database", id);

            UserEntity? foundUser = await _context.Users.FindAsync(id);
            if (foundUser == null) {
                _logger?.LogWarning("User with ID {UserId} not found for removal in database", id);
                throw new UserNotFoundException<Guid>(id);
            }
            
            _context.Users.Remove(foundUser);
            await _context.SaveChangesAsync();

            _logger?.LogDebug("User removed successfully from database with ID: {UserId}", id);
        }

        public async Task<bool> ExistsByEmail(string email)
        {
            _logger?.LogDebug("Checking if user with email {Email} exists in database", email);

            bool exists = await _context.Users.AnyAsync(user => user.email == email);
            _logger?.LogDebug("User with email {Email} exists: {Exists}", email, exists);
            return exists;
        }
    }
}