using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CartAPI.Models
{
    [Table("Cart", Schema = "dbo")]
    public class Cart
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("userid")]
        public string UserId { get; set; }

        [Column("product_code")]
        public string ProductCode { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }
    }
}