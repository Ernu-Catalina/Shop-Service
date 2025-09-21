namespace ShopService.Models
{
    public class ShopItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int Price { get; set; }
        public int Durability { get; set; }
        public string[] RolesVisible { get; set; } = Array.Empty<string>();
    }
}
