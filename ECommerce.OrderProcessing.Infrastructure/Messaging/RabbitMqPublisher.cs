using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ECommerce.OrderProcessing.Infrastructure.Messaging
{
    public class RabbitMqPublisher
    {
        private const string QueueName = "order-created";

        public void Publish<T>(T message)
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish(
                exchange: "",
                routingKey: QueueName,
                basicProperties: null,
                body: body);
        }
    }
}
