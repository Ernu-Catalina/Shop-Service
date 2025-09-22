using ShopService.Data;
using ShopService.Models;

namespace ShopService.Repositories
{
    public class ShopRepository
    {
        private readonly ShopDbContext _context;

        public ShopRepository(ShopDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ShopItem> GetItems() => _context.Items.ToList();

        public void AddItem(ShopItem item)
        {
            _context.Items.Add(item);
            _context.SaveChanges();
        }
    }
}
