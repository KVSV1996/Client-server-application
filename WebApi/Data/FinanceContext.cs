using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class FinanceContext : DbContext, IFinanceContext
    {
        public FinanceContext(DbContextOptions<FinanceContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<UserEntity> UserEntity { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
