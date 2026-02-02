using ECommerce.OrderProcessing.Application.Interfaces;

namespace ECommerce.OrderProcessing.Application.Jobs
{
    public class FakeEmailJob
    {
        private readonly IEmailService _emailService;

        public FakeEmailJob(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task EnviarEmailPedidoProcessado(long pedidoId, string email)
        {
            await _emailService.EnviarAsync(pedidoId, email);
        }
    }
}
