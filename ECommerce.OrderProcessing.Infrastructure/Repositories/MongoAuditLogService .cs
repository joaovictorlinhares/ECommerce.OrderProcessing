using ECommerce.OrderProcessing.Application.Interfaces;
using ECommerce.OrderProcessing.Application.Models;
using ECommerce.OrderProcessing.Infrastructure.Context;

namespace ECommerce.OrderProcessing.Infrastructure.Repositories
{
    public class MongoAuditLogService : IAuditLogService
    {
        private readonly MongoDbContext _context;

        public MongoAuditLogService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(OrderAuditLog log)
        {
            await _context.OrderAuditLogs.InsertOneAsync(log);
        }
    }
}
