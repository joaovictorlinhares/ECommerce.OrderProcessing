using ECommerce.OrderProcessing.Application.Interfaces;
using ECommerce.OrderProcessing.Domain.Entities;
using ECommerce.OrderProcessing.Domain.Enums;
using ECommerce.OrderProcessing.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.OrderProcessing.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task<Order> GetByIdAsync(long id)
            => await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.IsActive);

        public async Task<List<Order>> ListAsync(OrderStatus? status, int pageNumber, int pageSize, bool sortDescending)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                .Where(o => o.IsActive);

            if (status.HasValue)
                query = query.Where(o => o.Status == status);

            query = sortDescending
                ? query.OrderByDescending(o => o.CreatedAt)
                : query.OrderBy(o => o.CreatedAt);

            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }

}
