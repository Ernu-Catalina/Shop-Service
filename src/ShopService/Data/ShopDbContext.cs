using Microsoft.EntityFrameworkCore;
using ShopService.Models;

namespace ShopService.Data
{
    public class ShopDbContext : DbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

        public DbSet<ShopItem> Items { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShopItem>().ToTable("Items");
        }
    }
}
