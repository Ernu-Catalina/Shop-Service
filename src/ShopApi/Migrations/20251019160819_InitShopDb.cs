using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShopApi.Migrations
{
    /// <inheritdoc />
    public partial class InitShopDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_name = table.Column<string>(type: "text", nullable: false),
                    item_description = table.Column<string>(type: "text", nullable: false),
                    nr_of_uses = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<int>(type: "integer", nullable: false),
                    relevant_roles = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.item_id);
                });

            migrationBuilder.InsertData(
                table: "items",
                columns: new[] { "item_id", "item_description", "item_name", "nr_of_uses", "price", "relevant_roles" },
                values: new object[,]
                {
                    { 1, "Protects from one Vampire attack automatically per night", "Garlic", "1", 50, "1,2,3,4,5,6,8" },
                    { 2, "Can block a kill attempt or be used by Mafia to kill a player", "Silver Dagger", "1", 60, "1,2,3,4,5,6,8" },
                    { 3, "Required to save a player at night", "Healing Herbs", "3", 80, "4" },
                    { 4, "Reduces chance of being detected by Detective, Spy, or rumor by 60%", "Cloak", "unlimited", 100, "1,2,3,4,5,6,8" },
                    { 5, "Secretly eliminates a player", "Poison", "1", 60, "1" },
                    { 6, "Starts false rumors", "Rumor Scroll", "2", 80, "2" },
                    { 7, "Grants extra coins for completing tasks", "Coin Purse", "unlimited", 100, "1,2,3,4,5,6,8" },
                    { 8, "Heals the user once", "First Aid Kit", "1", 50, "1,2,3,4,5,6,8" },
                    { 9, "Blocks one Mafia kill attempt per night", "Bullet Proof Vest", "1", 60, "3,4,5,6,8" },
                    { 10, "Protects items from being examined by others", "Lockbox", "unlimited", 100, "1,2,3,4,5,6,8" },
                    { 11, "Temporarily changes player’s role appearance in investigations or rumors", "Disguise Kit", "1", 60, "1,2,3,4,5,6,8" },
                    { 12, "50% chance to block a Vampire or Mafia attack", "Lucky Charm", "3", 80, "1,2,3,4,5,6,8" },
                    { 13, "Eavesdrops on secret messages in Speakeasy or Back Alley", "Listening Device", "2", 60, "1,2,3,4,5,6,8" },
                    { 14, "Required for investigations", "Magnifying Glass", "3", 80, "2,3" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "items");
        }
    }
}
