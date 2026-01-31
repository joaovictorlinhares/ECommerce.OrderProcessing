using ECommerce.OrderProcessing.Domain.Entities;
using ECommerce.OrderProcessing.Domain.Enums;

namespace ECommerce.OrderProcessing.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order> GetByIdAsync(long id);
        Task<List<Order>> ListAsync(OrderStatus? status);
        Task UpdateAsync(Order order);
    }

}
