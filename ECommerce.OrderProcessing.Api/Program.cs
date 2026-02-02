using System.Text.Json.Serialization;
using ECommerce.OrderProcessing.Application.Interfaces;
using ECommerce.OrderProcessing.Application.Jobs;
using ECommerce.OrderProcessing.Application.Services;
using ECommerce.OrderProcessing.Infrastructure.Context;
using ECommerce.OrderProcessing.Infrastructure.Messaging;
using ECommerce.OrderProcessing.Infrastructure.Repositories;
using ECommerce.OrderProcessing.Infrastructure.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.UseInlineDefinitionsForEnums();
});

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IAuditLogService, MongoAuditLogService>();

builder.Services.AddSingleton<RabbitMqPublisher>();
builder.Services.AddHostedService<OrderCreatedConsumer>();

builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage();
});

builder.Services.AddHangfireServer();

builder.Services.AddScoped<IEmailService, FakeEmailService>();
builder.Services.AddScoped<FakeEmailJob>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard("/hangfire");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
