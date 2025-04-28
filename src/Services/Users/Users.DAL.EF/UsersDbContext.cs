using Microsoft.EntityFrameworkCore;
using Users.DAL.Entities;

namespace Users.DAL {
    public class UsersDbContext : DbContext {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        { 
            Database.EnsureCreated();
        }

        public DbSet<UserEntity> Users { get; set; }
    }
}