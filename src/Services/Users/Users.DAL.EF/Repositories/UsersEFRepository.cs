using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Users.DAL.Entities;
using Users.Domain.Exceptions;
using Users.Domain.Models;
using Users.Domain.Repositories;

namespace Users.DAL.Repositories {
    public class UsersEFRepository : IUsersRepository {
        private readonly UsersDbContext _context;
        private readonly IMapper _mapper;

        public UsersEFRepository(UsersDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task Add(User user)
        {
            UserEntity userEntity = _mapper.Map<UserEntity>(user);
            _context.Users.Add(userEntity);
            await _context.SaveChangesAsync();
        }

        public async Task<User> Get(Guid id)
        {
            UserEntity? foundUser = await _context.Users.FindAsync(id);
            User? user = _mapper.Map<User?>(foundUser);
            return user;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            IEnumerable<UserEntity> foundUsers = _context.Users.AsEnumerable();
            IEnumerable<User> users = _mapper.Map<IEnumerable<User>>(foundUsers);
            return users;
        }

        public async Task<User> Update(User user)
        {
            UserEntity? foundUser = await _context.Users.FindAsync(user.Id);
            if (foundUser == null) {
                throw new UserNotFoundException(user.Id);
            }
            _mapper.Map(user, foundUser);
            
            _context.Users.Update(foundUser);
            await _context.SaveChangesAsync();

            User updatedUser = _mapper.Map<User>(foundUser);
            return updatedUser;
        }
        
        public async Task Remove(Guid id)
        {
            UserEntity? foundUser = await _context.Users.FindAsync(id);
            if (foundUser == null) {
                throw new UserNotFoundException(id);
            }
            
            _context.Users.Remove(foundUser);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByEmail(string email)
        {
            return await _context.Users.AnyAsync(user => user.email == email);
        }
    }
}