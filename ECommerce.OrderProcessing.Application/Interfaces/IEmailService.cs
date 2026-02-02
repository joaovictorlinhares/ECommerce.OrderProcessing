namespace ECommerce.OrderProcessing.Application.Interfaces
{
    public interface IEmailService
    {
        Task EnviarAsync(long pedidoId, string email);
    }
}
