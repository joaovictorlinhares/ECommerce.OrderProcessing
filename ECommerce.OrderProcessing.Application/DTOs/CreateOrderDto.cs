using System.ComponentModel.DataAnnotations;

namespace ECommerce.OrderProcessing.Application.DTOs
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        public required string CustomerName { get; set; }

        [Required(ErrorMessage = "O e-mail do cliente é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado é inválido.")]
        public required string CustomerEmail { get; set; }

        [MinLength(1, ErrorMessage = "O pedido deve conter ao menos um item.")]
        public required List<OrderItemDto> Items { get; set; }
    }
}
