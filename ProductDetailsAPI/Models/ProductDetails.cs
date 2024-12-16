using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductDetailsAPI.Models
{
    [Table("ProductDetails", Schema = "dbo")]
    public class ProductDetails
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("product_code")]
        public string ProductCode { get; set; }

        [Column("size")]
        public string Size { get; set; }

        [Column("manufacturer")]
        public string Manufacturer { get; set; }

        [Column("specification")]
        public string Specification { get; set; }
    }
}
