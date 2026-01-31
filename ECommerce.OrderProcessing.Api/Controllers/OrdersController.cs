using ECommerce.OrderProcessing.Application.DTOs;
using ECommerce.OrderProcessing.Application.Interfaces;
using ECommerce.OrderProcessing.Domain.Enums;
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
        public async Task<IActionResult> GetById(long id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] OrderStatus? status)
            => Ok(await _service.ListAsync(status));

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            try
            {
                var id = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id }, null);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
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
    }
}
