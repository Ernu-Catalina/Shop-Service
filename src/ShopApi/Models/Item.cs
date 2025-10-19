using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApi.Models
{
    [Table("items")]
    public class Item
    {
        [Key]
        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("item_name")]
        public string ItemName { get; set; } = string.Empty;

        [Column("item_description")]
        public string ItemDescription { get; set; } = string.Empty;

        [Column("nr_of_uses")]
        public string NrOfUses { get; set; } = "1";

        [Column("price")]
        public int Price { get; set; }

        // stored as CSV "1,2,3" representing role ids
        [Column("relevant_roles")]
        public string RelevantRoles { get; set; } = string.Empty;
    }
}
