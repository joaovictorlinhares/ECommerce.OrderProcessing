using ECommerce.OrderProcessing.Domain.Entities;
using ECommerce.OrderProcessing.Domain.Enums;

namespace ECommerce.OrderProcessing.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Order> GetByIdAsync(long id);
        Task<List<Order>> ListAsync(OrderStatus? status);
    }
}
