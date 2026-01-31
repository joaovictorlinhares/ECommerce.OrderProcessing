using ECommerce.OrderProcessing.Application.Interfaces;
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
    }

}
