using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Models
{
    public class InventoryDto
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
    }
}