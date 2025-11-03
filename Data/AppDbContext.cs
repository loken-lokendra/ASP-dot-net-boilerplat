using Microsoft.EntityFrameworkCore;
using ASPDOTNETDEMO.Models;


namespace ASPDOTNETDEMO.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Full access" },
                new Role { Id = 2, Name = "User", Description = "Basic access" }
            );
        }
    }
}
