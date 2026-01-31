using System.ComponentModel.DataAnnotations;

namespace ECommerce.OrderProcessing.Application.DTOs
{
    public class UpdateOrderDto
    {
        [Required(ErrorMessage = "A lista de itens é obrigatória.")]
        [MinLength(1, ErrorMessage = "O pedido deve conter ao menos um item.")]
        public required List<OrderItemDto> Items { get; set; }
    }
}
