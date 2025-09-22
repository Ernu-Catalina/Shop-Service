namespace ShopService.Models
{
    public class ShopItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Durability { get; set; }
    }
}
