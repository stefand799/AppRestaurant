using System.Collections.Generic;
using System.Threading.Tasks;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.OrderItem
{
    public interface IOrderItemRepository : IBaseRepository<Models.OrderItem>
    {
        Task<IEnumerable<Models.OrderItem>> GetByOrderIdAsync(int orderId);
        Task<Models.OrderItem> GetByIdWithDetailsAsync(int id);
        Task AddRangeAsync(IEnumerable<Models.OrderItem> orderItems);
    }
}