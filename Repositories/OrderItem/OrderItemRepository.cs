using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppRestaurant.Data;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.OrderItem
{
    public class OrderItemRepository : BaseRepository<Models.OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(AppRestaurantDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Models.OrderItem>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Dish)
                .Include(oi => oi.Meal)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<Models.OrderItem> GetByIdWithDetailsAsync(int id)
        {
            return await _context.OrderItems
                .Include(oi => oi.Dish)
                .Include(oi => oi.Meal)
                .FirstOrDefaultAsync(oi => oi.Id == id);
        }

        public async Task AddRangeAsync(IEnumerable<Models.OrderItem> orderItems)
        {
            await _context.OrderItems.AddRangeAsync(orderItems);
        }
    }
}