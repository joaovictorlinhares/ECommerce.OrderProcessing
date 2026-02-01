using ECommerce.OrderProcessing.Application.DTOs;
using ECommerce.OrderProcessing.Application.Events;
using ECommerce.OrderProcessing.Application.Interfaces;
using ECommerce.OrderProcessing.Domain.Entities;
using ECommerce.OrderProcessing.Domain.Enums;
using ECommerce.OrderProcessing.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.OrderProcessing.Api.Controllers
{
    [ApiController]
    [Route("api/pedidos")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        [EndpointSummary("Obtém um pedido pelo ID")]
        [EndpointDescription("Retorna os detalhes de um pedido existente")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet]
        [EndpointSummary("Lista pedidos")]
        [EndpointDescription("Retorna a lista de pedidos com filtro opcional por status")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] OrderStatus? status)
            => Ok(await _service.ListAsync(status));

        [HttpPost]
        [EndpointSummary("Cria um novo pedido")]
        [EndpointDescription("Cria um pedido e retorna o Id")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, [FromServices] RabbitMqPublisher rabbitMq)
        {
            try
            {
                var id = await _service.CreateAsync(dto);

                rabbitMq.Publish(new OrderCreatedEvent { Id = id });

                return CreatedAtAction(nameof(GetById), new { id }, null);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [EndpointSummary("Atualiza um pedido")]
        [EndpointDescription("Atualiza os dados de um pedido existente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(long id, UpdateOrderDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [EndpointSummary("Cancela um pedido")]
        [EndpointDescription("Realiza o cancelamento de um pedido existente")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Cancel(long id)
        {
            await _service.CancelAsync(id);
            return NoContent();
        }
    }
}
