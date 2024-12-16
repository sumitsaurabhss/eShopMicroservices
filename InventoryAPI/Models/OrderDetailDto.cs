namespace InventoryAPI.Models
{
    public class OrderDetailDto
    {
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
    }
}