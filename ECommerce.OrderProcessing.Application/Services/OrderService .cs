using ECommerce.OrderProcessing.Application.DTOs;
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

        public async Task<long> CreateAsync(CreateOrderDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new ArgumentException("O pedido deve conter ao menos um item.");

            if (dto.Items.Any(i => i.UnitPrice <= 0 || i.Quantity <= 0))
                throw new ArgumentException("Não é permitido registrar itens com valor unitário ou quantidade igual ou menor que zero.");

            var order = new Order
            {
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                Status = OrderStatus.Recebido,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Items = dto.Items.Select(i => new OrderItem
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

            await _repository.AddAsync(order);
            return order.Id;
        }
    }

}
