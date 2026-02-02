using ECommerce.OrderProcessing.Application.Interfaces;

namespace ECommerce.OrderProcessing.Infrastructure.Services
{
    public class FakeEmailService : IEmailService
    {
        public Task EnviarAsync(long pedidoId, string email)
        {
            Console.WriteLine($"[FAKE EMAIL] Pedido {pedidoId} processado com sucesso. Destinatário: {email}");

            return Task.CompletedTask;
        }
    }
}
