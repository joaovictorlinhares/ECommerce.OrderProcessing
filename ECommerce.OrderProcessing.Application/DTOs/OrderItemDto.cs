using System.ComponentModel.DataAnnotations;

namespace ECommerce.OrderProcessing.Application.DTOs
{
    public class OrderItemDto
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [MinLength(2, ErrorMessage = "O nome do produto deve ter ao menos 2 caracteres.")]
        public required string ProductName { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "O valor unitário é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor unitário deve ser maior que zero.")]
        public decimal UnitPrice { get; set; }
    }

}
