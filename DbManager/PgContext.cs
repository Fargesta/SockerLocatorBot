using DbManager.Models;
using Microsoft.EntityFrameworkCore;

namespace DbManager
{
    public class PgContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; } = null!;
        public DbSet<RoleModel> Roles { get; set; } = null!;
        public DbSet<LocationModel> Locations { get; set; } = null!;
        public DbSet<ImageModel> Images { get; set; } = null!;

        public PgContext(DbContextOptions<PgContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PgContext).Assembly);
        }
    }
}
