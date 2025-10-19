using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;

namespace ShopApi.Repos
{
    public class ItemRepository
    {
        private readonly ShopDbContext _db;
        private readonly ILogger<ItemRepository> _logger;

        public ItemRepository(ShopDbContext db, ILogger<ItemRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<Item>> GetAllAsync()
        {
            return await _db.Items.AsNoTracking().ToListAsync();
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            return await _db.Items.FindAsync(id);
        }

        public async Task<List<Item>> GetByRoleAsync(int roleId)
        {
            var items = await _db.Items.AsNoTracking().ToListAsync();
            return items.Where(i =>
            {
                var roles = i.RelevantRoles.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
                return roles.Any(r => int.TryParse(r, out var rid) && rid == roleId);
            }).ToList();
        }
    }
}
