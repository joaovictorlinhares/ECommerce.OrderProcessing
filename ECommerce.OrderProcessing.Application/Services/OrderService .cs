using ECommerce.OrderProcessing.Application.Interfaces;
using ECommerce.OrderProcessing.Domain.Entities;
using ECommerce.OrderProcessing.Domain.Enums;
using ECommerce.OrderProcessing.Infrastructure.Repositories;

namespace ECommerce.OrderProcessing.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public Task<Order> GetByIdAsync(long id)
            => _repository.GetByIdAsync(id);

        public Task<List<Order>> ListAsync(OrderStatus? status)
            => _repository.ListAsync(status);
    }

}
