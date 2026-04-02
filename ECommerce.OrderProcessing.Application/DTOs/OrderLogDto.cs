using ECommerce.OrderProcessing.Domain.Enums;

namespace ECommerce.OrderProcessing.Application.DTOs
{
    public class OrderLogDto
    {
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
