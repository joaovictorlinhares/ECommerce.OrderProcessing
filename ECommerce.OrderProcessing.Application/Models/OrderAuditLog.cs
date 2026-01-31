namespace ECommerce.OrderProcessing.Application.Models
{
    public class OrderAuditLog
    {
        public long OrderId { get; set; }
        public string Action { get; set; } = "UPDATE";
        public object Before { get; set; } = default!;
        public object After { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
