using ECommerce.OrderProcessing.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECommerce.OrderProcessing.Application.Models
{
    public class OrderAuditLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public long OrderId { get; set; }
        public required string CorrelationId { get; set; }
        public string Action { get; set; } = "UPDATE";
        public required OrderStatus OrderStatusAfter { get; set; }
        public object Before { get; set; } = default!;
        public object After { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
