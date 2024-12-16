using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Models
{
    [Table("Product", Schema = "dbo")]
    public class Product
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("product_code")]
        public string ProductCode { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("image")]
        public string Image { get; set; }

        [Column("category")]
        public string Category { get; set; }

        [Column("stock")]
        public bool Stock { get; set; }
    }
}
