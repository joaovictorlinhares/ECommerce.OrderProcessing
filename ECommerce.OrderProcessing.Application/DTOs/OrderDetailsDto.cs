using ECommerce.OrderProcessing.Domain.Enums;

namespace ECommerce.OrderProcessing.Application.DTOs
{
    public class OrderDetailsDto
    {
        public long Id { get; set; }

        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public Guid CorrelationId { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();

        public List<OrderLogDto> Logs { get; set; } = new();
    }
}
