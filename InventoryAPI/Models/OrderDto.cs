namespace InventoryAPI.Models
{
    public class OrderDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalItems { get; set; }
        public DateTime OrderedOn { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
    }
}
