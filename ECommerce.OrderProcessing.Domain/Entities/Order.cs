using ECommerce.OrderProcessing.Domain.Enums;

namespace ECommerce.OrderProcessing.Domain.Entities
{
    public class Order
    {
        public long Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
}
