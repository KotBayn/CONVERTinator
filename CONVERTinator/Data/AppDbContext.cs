using Microsoft.EntityFrameworkCore;
using CONVERTinator.Domain.Entities;
using System.IO;

namespace CONVERTinator.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<CachedRate> CachedRates { get; set; }
        public DbSet<UserSettings> Settings { get; set; }
        public AppDbContext()
        {
            Directory.CreateDirectory("Data");
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Data/convertinator.db");
        }
    }
}