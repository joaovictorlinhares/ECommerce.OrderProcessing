using ECommerce.OrderProcessing.Application.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ECommerce.OrderProcessing.Infrastructure.Context
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoDb:ConnectionString"]);
            _database = client.GetDatabase(configuration["MongoDb:Database"]);
        }

        public IMongoCollection<OrderAuditLog> OrderAuditLogs =>
            _database.GetCollection<OrderAuditLog>("OrderAuditLogs");
    }
}
