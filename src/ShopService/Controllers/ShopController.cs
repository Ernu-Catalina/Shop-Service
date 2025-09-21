using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/shop")]
public class ShopController : ControllerBase
{
    private static readonly List<Item> Items = new()
    {
        new Item{ Id=1, Title="Garlic", Description="Blocks Vampire (1 use)", Price=50, Durability=1, RolesVisible = new[]{"Citizen","Doctor","Detective"} },
        new Item{ Id=2, Title="Healing Herbs", Description="Doctor only (3 uses)", Price=80, Durability=3, RolesVisible = new[]{"Doctor"} }
    };

    // very small in-memory balances for Grade 1 testing
    private static readonly Dictionary<int, int> CharacterBalances = new() { [101] = 80, [102] = 50 };

    [HttpGet("items")]
    public IActionResult GetItems() => Ok(Items);

    public class PurchaseRequest { public int CharacterId { get; set; } public int ItemId { get; set; } public int Quantity { get; set; } }

    [HttpPost("purchase")]
    public IActionResult Purchase([FromBody] PurchaseRequest req)
    {
        var item = Items.FirstOrDefault(i => i.Id == req.ItemId);
        if (item == null) return BadRequest(new { error = "Item not found" });

        var balance = CharacterBalances.ContainsKey(req.CharacterId) ? CharacterBalances[req.CharacterId] : 0;
        var cost = item.Price * Math.Max(1, req.Quantity);
        if (balance < cost) return BadRequest(new { error = "Insufficient funds" });

        CharacterBalances[req.CharacterId] = balance - cost;

        return Ok(new
        {
            purchaseId = Guid.NewGuid().ToString(),
            itemsAdded = new[] { new { inventoryItemId = Guid.NewGuid().ToString(), itemId = item.Id, remainingDurability = item.Durability } },
            newBalance = CharacterBalances[req.CharacterId]
        });
    }
}

public class Item
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int Price { get; set; }
    public int Durability { get; set; }
    public string[] RolesVisible { get; set; } = Array.Empty<string>();
}
