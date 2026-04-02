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

        public async Task<OrderDetailsDto> GetByIdAsync(long id)
        {
            var order = await _repository.GetByIdAsync(id);

            var logs = await _auditLogService.GetByOrderIdAsync(id);

            return new OrderDetailsDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CorrelationId = order.CorrelationId,
                CreatedAt = order.CreatedAt,

                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),

                Logs = logs.Select(l => new OrderLogDto
                {
                    Status = l.OrderStatusAfter,
                    CreatedAt = l.CreatedAt,
                }).ToList()
            };
        }
            

        public Task<List<Order>> ListAsync(OrderStatus? status, int pageNumber, int pageSize, bool sortDescending)
            => _repository.ListAsync(status, pageNumber, pageSize, sortDescending);

        public async Task<long> CreateAsync(CreateOrderDto dto)
        {
            var order = new Order
            {
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                Status = OrderStatus.Recebido,
                CorrelationId = Guid.NewGuid(),
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

            await _auditLogService.LogAsync(new OrderAuditLog
            {
                OrderId = order.Id,
                OrderStatusAfter = order.Status,
                CorrelationId = order.CorrelationId.ToString(),
                Action = "CREATE",
                After = new
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
                }
            });

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
                OrderStatusAfter = order.Status,
                CorrelationId = order.CorrelationId.ToString(),
                Before = before,
                After = after
            });
        }

        public async Task<Order> ProcessAsync(long id)
        {
            var order = await _repository.GetByIdAsync(id);

            order.Status = OrderStatus.Processado;

            await _repository.UpdateAsync(order);

            await _auditLogService.LogAsync(new OrderAuditLog
            {
                Action = "PROCESS",
                OrderId = order.Id,
                OrderStatusAfter = order.Status,
                CorrelationId = order.CorrelationId.ToString(),
            });

            return order;
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
                OrderStatusAfter = order.Status,
                CorrelationId = order.CorrelationId.ToString(),
                Before = before
            });
        }
    }

}
