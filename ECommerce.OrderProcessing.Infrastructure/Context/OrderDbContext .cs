using ECommerce.OrderProcessing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.OrderProcessing.Infrastructure.Context
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.HasMany(o => o.Items)
                      .WithOne()
                      .HasForeignKey(i => i.OrderId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
