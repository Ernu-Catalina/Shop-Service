using System.Collections.Generic;

namespace ShopService
{
    // In-memory repository for shop items
    public class ShopRepository
    {
        private readonly List<Item> _items = new();

        public IEnumerable<Item> GetItems() => _items;

        public void AddItem(Item item) => _items.Add(item);
    }

    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Durability { get; set; }
    }
}
