using ECommerce.OrderProcessing.Application.Models;

namespace ECommerce.OrderProcessing.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(OrderAuditLog log);
    }
}
