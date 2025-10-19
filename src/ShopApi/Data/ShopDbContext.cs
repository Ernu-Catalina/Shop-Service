using Microsoft.EntityFrameworkCore;
using ShopApi.Models;

namespace ShopApi.Data
{
    public class ShopDbContext : DbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // seed initial data
            modelBuilder.Entity<Item>().HasData(
                new Item { ItemId = 1, ItemName = "Garlic", ItemDescription = "Protects from one Vampire attack automatically per night", NrOfUses = "1", Price = 50, RelevantRoles = "1,2,3,4,5,6,8" },
                new Item { ItemId = 2, ItemName = "Silver Dagger", ItemDescription = "Can block a kill attempt or be used by Mafia to kill a player", NrOfUses = "1", Price = 60, RelevantRoles = "1,2,3,4,5,6,8" },
                new Item { ItemId = 3, ItemName = "Healing Herbs", ItemDescription = "Required to save a player at night", NrOfUses = "3", Price = 80, RelevantRoles = "4" },
                new Item { ItemId = 4, ItemName = "Cloak", ItemDescription = "Reduces chance of being detected by Detective, Spy, or rumor by 60%", NrOfUses = "unlimited", Price = 100, RelevantRoles = "1,2,3,4,5,6,8" },
                new Item { ItemId = 5, ItemName = "Poison", ItemDescription = "Secretly eliminates a player", NrOfUses = "1", Price = 60, RelevantRoles = "1" },
                new Item { ItemId = 6, ItemName = "Rumor Scroll", ItemDescription = "Starts false rumors", NrOfUses = "2", Price = 80, RelevantRoles = "2" },
                new Item { ItemId = 7, ItemName = "Coin Purse", ItemDescription = "Grants extra coins for completing tasks", NrOfUses = "unlimited", Price = 100, RelevantRoles = "1,2,3,4,5,6,8" },
                new Item { ItemId = 8, ItemName = "First Aid Kit", ItemDescription = "Heals the user once", NrOfUses = "1", Price = 50, RelevantRoles = "1,2,3,4,5,6,8" },
                new Item { ItemId = 9, ItemName = "Bullet Proof Vest", ItemDescription = "Blocks one Mafia kill attempt per night", NrOfUses = "1", Price = 60, RelevantRoles = "3,4,5,6,8" },
                new Item { ItemId = 10, ItemName = "Lockbox", ItemDescription = "Protects items from being examined by others", NrOfUses = "unlimited", Price = 100, RelevantRoles = "1,2,3,4,5,6,8" },
                new Item { ItemId = 11, ItemName = "Disguise Kit", ItemDescription = "Temporarily changes player’s role appearance in investigations or rumors", NrOfUses = "1", Price = 60, RelevantRoles = "1,2,3,4,5,6,8" },
                new Item { ItemId = 12, ItemName = "Lucky Charm", ItemDescription = "50% chance to block a Vampire or Mafia attack", NrOfUses = "3", Price = 80, RelevantRoles = "1,2,3,4,5,6,8" },
                new Item { ItemId = 13, ItemName = "Listening Device", ItemDescription = "Eavesdrops on secret messages in Speakeasy or Back Alley", NrOfUses = "2", Price = 60, RelevantRoles = "1,2,3,4,5,6,8" },
                new Item { ItemId = 14, ItemName = "Magnifying Glass", ItemDescription = "Required for investigations", NrOfUses = "3", Price = 80, RelevantRoles = "2,3" }
            );
        }
    }
}
