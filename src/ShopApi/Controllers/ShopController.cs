using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using ShopApi.Models;
using ShopApi.Repos;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [ApiController]
    [Route("shop")]
    public class ShopController : ControllerBase
    {
        private readonly ItemRepository _repo;
        private readonly IDistributedCache _cache;
        private readonly ILogger<ShopController> _logger;
        private readonly ICharacterClient _characterClient;
        private readonly int _cacheTtl;

        public ShopController(ItemRepository repo, IDistributedCache cache, ILogger<ShopController> logger, ICharacterClient characterClient, IConfiguration config)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;
            _characterClient = characterClient;
            _cacheTtl = int.TryParse(config["CacheDefaultTTL"], out var t) ? t : 15;
        }

        // 1. Retrieve all shop items
        [HttpGet("items")]
        public async Task<IActionResult> GetAllItems()
        {
            var cacheKey = "shop:items:all";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("Cache hit for all items");
                return Ok(JsonSerializer.Deserialize<List<Item>>(cached));
            }

            var items = await _repo.GetAllAsync();
            var payload = JsonSerializer.Serialize(items);
            await _cache.SetStringAsync(cacheKey, payload, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_cacheTtl) });
            _logger.LogInformation("Cache miss - stored all items");
            return Ok(items);
        }

        // 2. Get available items for role (always 5 items, role-based)
        [HttpGet("items/available/{role}")]
        public async Task<IActionResult> GetAvailableItemsForRole(string role)
        {
            var cacheKey = $"shop:items:role:{role.ToLower()}";
            var cached = await _cache.GetStringAsync(cacheKey);
            List<Item> itemsForRole;

            if (cached != null)
            {
                itemsForRole = JsonSerializer.Deserialize<List<Item>>(cached)!;
            }
            else
            {
                var allItems = await _repo.GetAllAsync();
                itemsForRole = new List<Item>();

                // Add mandatory items
                switch (role.ToLower())
                {
                    case "doctor":
                        AddIfExists(itemsForRole, allItems, "Healing Herbs");
                        break;
                    case "mafia":
                        AddIfExists(itemsForRole, allItems, "Poison");
                        break;
                    case "detective":
                    case "spy":
                        AddIfExists(itemsForRole, allItems, "Magnifying Glass");
                        break;
                }

                // Add items according to role restrictions
                foreach (var item in allItems)
                {
                    if (itemsForRole.Contains(item)) continue;

                    switch (item.ItemName)
                    {
                        case "Garlic":
                            if (role.ToLower() != "vampire") itemsForRole.Add(item);
                            break;
                        case "Healing Herbs":
                            if (role.ToLower() == "doctor") itemsForRole.Add(item);
                            break;
                        case "Poison":
                            if (role.ToLower() == "mafia") itemsForRole.Add(item);
                            break;
                        case "Rumor Scroll":
                            if (role.ToLower() == "spy") itemsForRole.Add(item);
                            break;
                        case "Bullet Proof Vest":
                            if (role.ToLower() != "mafia" && role.ToLower() != "spy") itemsForRole.Add(item);
                            break;
                        case "Magnifying Glass":
                            if (role.ToLower() == "detective" || role.ToLower() == "spy") itemsForRole.Add(item);
                            break;
                        default:
                            itemsForRole.Add(item);
                            break;
                    }
                }

                // Randomize and take exactly 5 items
                var rng = new Random();
                itemsForRole = itemsForRole.OrderBy(x => rng.Next()).Take(5).ToList();

                // Cache
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(itemsForRole), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_cacheTtl) });
            }

            return Ok(itemsForRole.Select(i => new
            {
                i.ItemId,
                i.ItemName,
                i.ItemDescription,
                i.Price,
                i.RelevantRoles,
                i.NrOfUses
            }));
        }

        private void AddIfExists(List<Item> list, List<Item> allItems, string name)
        {
            var item = allItems.FirstOrDefault(i => i.ItemName == name);
            if (item != null) list.Add(item);
        }

        // 3. Get player role - proxied to Character service
        [HttpGet("character/{characterId}/role")]
        public async Task<IActionResult> GetPlayerRole(int characterId)
        {
            var role = await _characterClient.GetRoleAsync(characterId);
            if (role == null) return NotFound(new { message = "Character or role not found" });
            return Ok(new { role_id = role });
        }

        // 4. Get player currency - proxied to Character service
        [HttpGet("character/{characterId}/currency")]
        public async Task<IActionResult> GetPlayerCurrency(int characterId)
        {
            var amount = await _characterClient.GetCurrencyAsync(characterId);
            if (amount == null) return NotFound(new { message = "Character not found" });
            return Ok(new { currency_amount = amount });
        }

        // 5. Purchase an item
        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request)
        {
            if (request == null) return BadRequest(new { success = false, message = "Invalid request" });

            var amount = await _characterClient.GetCurrencyAsync(request.CharacterId);
            if (amount == null) return NotFound(new { success = false, message = "Character not found" });

            var item = await _repo.GetByIdAsync(request.ItemId);
            if (item == null) return NotFound(new { success = false, message = "Item not found" });

            if (amount < item.Price)
            {
                return BadRequest(new { success = false, message = "Not enough currency" });
            }

            var addResp = await _characterClient.AddItemToInventoryAsync(request.CharacterId, request.ItemId);
            if (!addResp.Success)
            {
                return StatusCode(500, new { success = false, message = "Failed to add to inventory: " + addResp.Message });
            }

            return Ok(new { success = true, message = "Purchase successful" });
        }
    }

    public class PurchaseRequest
    {
        public int CharacterId { get; set; }
        public int ItemId { get; set; }
    }
}
