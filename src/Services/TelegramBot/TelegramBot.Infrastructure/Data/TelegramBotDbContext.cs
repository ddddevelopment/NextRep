using Microsoft.EntityFrameworkCore;
using TelegramBot.Domain.Models;

namespace TelegramBot.Infrastructure.Data
{
    public class TelegramBotDbContext : DbContext
    {
        public TelegramBotDbContext(DbContextOptions<TelegramBotDbContext> options) : base(options)
        {
        }

        public DbSet<TelegramUser> TelegramUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TelegramUser>(entity =>
            {
                entity.HasKey(e => e.TelegramId);
                
                entity.Property(e => e.TelegramId)
                    .ValueGeneratedNever();
                
                entity.Property(e => e.FirstName)
                    .HasMaxLength(255);
                
                entity.Property(e => e.LastName)
                    .HasMaxLength(255);
                
                entity.Property(e => e.Username)
                    .HasMaxLength(255);
                
                entity.Property(e => e.TemporaryData)
                    .HasMaxLength(4000);
                
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
                
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Username);
            });
        }
    }
}