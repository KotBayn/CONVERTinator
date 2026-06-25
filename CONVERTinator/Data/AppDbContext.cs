using Microsoft.EntityFrameworkCore;
using System.IO;
using CONVERTinator.Domain.Entities;

namespace CONVERTinator.Data
{
    public class AppDbContext : DbContext
    {
        // Tables
        public DbSet<CachedRate> CachedRates { get; set; }
        public DbSet<UserSettings> Settings { get; set; }

        public AppDbContext()
        {
            Directory.CreateDirectory("Data");
            // auto-migrate database 
            Database.Migrate();
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=Data/convertinator.db");
            }
        }
    }
}