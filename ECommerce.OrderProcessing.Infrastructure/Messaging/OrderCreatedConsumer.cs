using System.Text;
using System.Text.Json;
using ECommerce.OrderProcessing.Application.Events;
using ECommerce.OrderProcessing.Application.Interfaces;
using ECommerce.OrderProcessing.Application.Jobs;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ECommerce.OrderProcessing.Infrastructure.Messaging
{
    public class OrderCreatedConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OrderCreatedConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var factory = new ConnectionFactory
                    {
                        HostName = "rabbitmq",
                        UserName = "guest",
                        Password = "guest"
                    };

                    var connection = factory.CreateConnection();
                    var channel = connection.CreateModel();

                    channel.QueueDeclare(
                        queue: "order-created",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += async (_, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        var orderEvent =
                            JsonSerializer.Deserialize<OrderCreatedEvent>(message);

                        if (orderEvent == null)
                            return;

                        using var scope = _serviceProvider.CreateScope();
                        var orderService = scope.ServiceProvider
                            .GetRequiredService<IOrderService>();

                        var order = await orderService.ProcessAsync(orderEvent.Id);

                        BackgroundJob.Enqueue<FakeEmailJob>(
                            job => job.EnviarEmailPedidoProcessado(
                                order.Id,
                                order.CustomerEmail
                            )
                        );
                    };

                    channel.BasicConsume(
                        queue: "order-created",
                        autoAck: true,
                        consumer: consumer);

                    Console.WriteLine("RabbitMQ consumer conectado com sucesso.");

                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao conectar no RabbitMQ. Tentando novamente em 5s. {ex.Message}");

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }

    }
}
