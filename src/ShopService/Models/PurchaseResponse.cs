namespace ShopService.Models
{
    public class PurchaseResponse
    {
        public int PurchaseId { get; set; }
        public object[] ItemsAdded { get; set; } = Array.Empty<object>();
        public int NewBalance { get; set; }
    }
}
