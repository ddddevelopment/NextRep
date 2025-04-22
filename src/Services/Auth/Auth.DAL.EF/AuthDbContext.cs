using Auth.DAL.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace Auth.DAL.EF;

public class AuthDbContext : DbContext {
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) {}

    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
}