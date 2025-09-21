namespace ShopService.Models
{
    public class PurchaseRequest
    {
        public int CharacterId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
