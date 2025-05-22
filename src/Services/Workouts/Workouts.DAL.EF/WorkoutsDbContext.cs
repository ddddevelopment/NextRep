using Microsoft.EntityFrameworkCore;
using Workouts.DAL.Entities;

namespace Workouts.DAL
{
    public class WorkoutsDbContext : DbContext
    {
        public WorkoutsDbContext(DbContextOptions<WorkoutsDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<WorkoutEntity> Workouts { get; set; }
    }
}
