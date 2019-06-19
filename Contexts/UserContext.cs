using Microsoft.EntityFrameworkCore;

namespace authService.Contexts
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Model.Db.User>()
                .ToTable("users");
        }

        public DbSet<Model.Db.User> Users { get; set; }
    }
}
