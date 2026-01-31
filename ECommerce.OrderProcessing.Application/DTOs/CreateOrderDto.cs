namespace ECommerce.OrderProcessing.Application.DTOs
{
    public class CreateOrderDto
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
