using System.Diagnostics.Metrics;
using ECommerce.OrderProcessing.Application.DTOs;
using ECommerce.OrderProcessing.Application.Interfaces;
using ECommerce.OrderProcessing.Application.Models;
using ECommerce.OrderProcessing.Domain.Entities;
using ECommerce.OrderProcessing.Domain.Enums;

namespace ECommerce.OrderProcessing.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IAuditLogService _auditLogService;

        public OrderService(IOrderRepository repository, IAuditLogService auditLogService)
        {
            _repository = repository;
            _auditLogService = auditLogService;
        }

        public Task<Order> GetByIdAsync(long id)
            => _repository.GetByIdAsync(id);

        public Task<List<Order>> ListAsync(OrderStatus? status)
            => _repository.ListAsync(status);

        public async Task<long> CreateAsync(CreateOrderDto dto)
        {
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

            if (order.TotalAmount <= 0)
                throw new InvalidOperationException("O pedido não pode ter valor total igual ou menor que zero.");

            await _repository.AddAsync(order);
            return order.Id;
        }

        public async Task UpdateAsync(long id, UpdateOrderDto dto)
        {
            var order = await _repository.GetByIdAsync(id);

            if (order == null)
                throw new KeyNotFoundException("O pedido informado não foi encontrado.");

            if (order.Status != OrderStatus.Recebido)
                throw new InvalidOperationException("Não é possível alterar um pedido já processado");

            var before = new
            {
                order.Id,
                order.Status,
                order.TotalAmount,
                Items = order.Items.Select(i => new
                {
                    i.ProductName,
                    i.Quantity,
                    i.UnitPrice
                }).ToList()
            };

            order.Items.Clear();
            order.Items = dto.Items.Select(i => new OrderItem
            {
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();

            order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

            await _repository.UpdateAsync(order);

            var after = new
            {
                order.Id,
                order.Status,
                order.TotalAmount,
                Items = order.Items.Select(i => new
                {
                    i.ProductName,
                    i.Quantity,
                    i.UnitPrice
                }).ToList()
            };

            await _auditLogService.LogAsync(new OrderAuditLog
            {
                OrderId = order.Id,
                Before = before,
                After = after
            });
        }

        public async Task CancelAsync(long id)
        {
            var order = await _repository.GetByIdAsync(id);

            var before = new
            {
                order.Id,
                order.Status,
                order.TotalAmount,
                Items = order.Items.Select(i => new
                {
                    i.ProductName,
                    i.Quantity,
                    i.UnitPrice
                }).ToList()
            };

            order.Status = OrderStatus.Cancelado;
            order.IsActive = false;
            await _repository.UpdateAsync(order);

            await _auditLogService.LogAsync(new OrderAuditLog
            {
                Action = "SOFT DELETE",
                OrderId = order.Id,
                Before = before
            });
        }
    }

}
