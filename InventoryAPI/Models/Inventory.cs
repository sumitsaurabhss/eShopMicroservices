using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.Models
{
    [Table("Inventory", Schema = "dbo")]
    public class Inventory
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("product_code")]
        public string ProductCode { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }
    }
}