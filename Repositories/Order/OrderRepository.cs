using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppRestaurant.Data;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.Order
{
    public class OrderRepository : BaseRepository<Models.Order>, IOrderRepository
    {
        public OrderRepository(AppRestaurantDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Models.Order>> GetAllWithDetailsAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Dish)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Meal)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Models.Order> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Dish)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Meal)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Models.Order>> GetByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Dish)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Meal)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Order>> GetActiveOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Dish)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Meal)
                .Where(o => o.Status != "Completed" && o.Status != "Cancelled")
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Order>> GetUserActiveOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Dish)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Meal)
                .Where(o => o.UserId == userId && o.Status != "Completed" && o.Status != "Cancelled")
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<string> GenerateOrderCodeAsync()
        {
            // Generate a unique order code with format ORD-YYYYMMDD-XXXX
            string prefix = "ORD-" + DateTime.Now.ToString("yyyyMMdd") + "-";
            
            // Find the latest order code for today
            var latestOrder = await _context.Orders
                .Where(o => o.OrderCode.StartsWith(prefix))
                .OrderByDescending(o => o.OrderCode)
                .FirstOrDefaultAsync();
            
            int nextNumber = 1;
            
            if (latestOrder != null)
            {
                string lastNumber = latestOrder.OrderCode.Substring(prefix.Length);
                
                if (int.TryParse(lastNumber, out int lastValue))
                {
                    nextNumber = lastValue + 1;
                }
            }
            
            return prefix + nextNumber.ToString("D4");
        }

        public async Task<int> GetOrderCountForUserInTimeRangeAsync(int userId, int days)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            
            return await _context.Orders
                .CountAsync(o => o.UserId == userId && o.OrderDate >= cutoffDate);
        }

        public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            
            if (order != null)
            {
                order.Status = newStatus;
                
                // If the order is being prepared, update the estimated delivery time
                if (newStatus == "Preparing")
                {
                    // Set estimated delivery time to 30 minutes from now
                    order.EstimatedDeliveryTime = DateTime.Now.AddMinutes(30);
                }
                
                await _context.SaveChangesAsync();
            }
        }
    }
}