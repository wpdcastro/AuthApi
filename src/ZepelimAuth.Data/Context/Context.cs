using Microsoft.EntityFrameworkCore;
using ZAuth.Database.EntityConfig;
using ZepelimAuth.Business.Models;

namespace ZepelimAuth.Database
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfig());

            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);
            // base.OnModelCreating(modelBuilder);
        }
    }
}
