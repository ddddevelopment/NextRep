using Microsoft.EntityFrameworkCore;
using Workouts.DAL.EF.Entities;

namespace Workouts.DAL
{
    public class WorkoutsDbContext : DbContext
    {
        public WorkoutsDbContext(DbContextOptions<WorkoutsDbContext> options) : base(options)
        {
            Database.EnsureCreated(); 
        }

        public DbSet<WorkoutEntity> Workouts { get; set; }
        public DbSet<ExerciseEntity> Exercises { get; set; }
        public DbSet<SetEntity> Sets { get; set; }
        public DbSet<ExerciseInfoEntity> ExerciseInfos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExerciseInfoEntity>().Property(e => e.muscle_group).HasConversion<string>();
        }
    }
}
