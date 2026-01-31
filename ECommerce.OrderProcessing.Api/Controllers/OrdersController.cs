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
    }
}
