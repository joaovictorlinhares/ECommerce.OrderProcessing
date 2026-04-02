using ECommerce.OrderProcessing.Application.Interfaces;
using ECommerce.OrderProcessing.Application.Models;
using ECommerce.OrderProcessing.Infrastructure.Context;
using MongoDB.Driver;

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

        public async Task<List<OrderAuditLog>> GetByOrderIdAsync(long orderId)
        {
            return await _context.OrderAuditLogs
                .Find(x => x.OrderId == orderId)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
