using ECommerce.OrderProcessing.Application.Interfaces;
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
    }
}
