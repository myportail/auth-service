using System;
using Microsoft.EntityFrameworkCore;
//using MySql.Data.EntityFrameworkCore;

namespace authService.Contexts
{
    public class UsersDbContext : DbContext
    {
        public DbSet<Model.Db.User> Users { get; set; }

        public UsersDbContext(DbContextOptions options) : base(options)
        {
            
        }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
////            optionsBuilder.UseMySQL("server=localhost;database=auth;user=authUser;password=igQFUwjZZyxgken7gcKg*gTu");
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Model.Db.User>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Name)
                    .IsRequired();
                entity.HasIndex(b => b.Name)
                    .IsUnique();
            });
        }
    }
}

