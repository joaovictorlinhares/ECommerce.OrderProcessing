using ECommerce.OrderProcessing.Application.DTOs;
using ECommerce.OrderProcessing.Application.Interfaces;
using ECommerce.OrderProcessing.Application.Services;
using ECommerce.OrderProcessing.Domain.Entities;
using ECommerce.OrderProcessing.Domain.Enums;
using Moq;

namespace ECommerce.OrderProcessing.Application.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly Mock<IAuditLogService> _auditLogMock;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            _auditLogMock = new Mock<IAuditLogService>();
            _service = new OrderService(_repositoryMock.Object, _auditLogMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_TotalAmount_Is_Zero()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                CustomerName = "João",
                CustomerEmail = "joao@email.com",
                Items = new List<OrderItemDto>
                {
                    new()
                    {
                        ProductName = "Produto inválido",
                        Quantity = 1,
                        UnitPrice = 0
                    }
                }
            };

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateAsync(dto)
            );

            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Order>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_When_Order_Status_Is_Not_Recebido()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                Status = OrderStatus.Processado,
                Items = new List<OrderItem>
                {
                    new()
                    {
                        ProductName = "Produto A",
                        Quantity = 1,
                        UnitPrice = 10
                    }
                }
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(order);

            var dto = new UpdateOrderDto
            {
                Items = new List<OrderItemDto>
                {
                    new()
                    {
                        ProductName = "Produto B",
                        Quantity = 2,
                        UnitPrice = 20
                    }
                }
            };

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.UpdateAsync(1, dto)
            );

            _repositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<Order>()),
                Times.Never
            );
        }
    }
}