using System.Collections.Generic;
using System.Threading.Tasks;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.Order
{
    public interface IOrderRepository : IBaseRepository<Models.Order>
    {
        Task<IEnumerable<Models.Order>> GetAllWithDetailsAsync();
        Task<Models.Order> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Models.Order>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Models.Order>> GetActiveOrdersAsync();
        Task<IEnumerable<Models.Order>> GetUserActiveOrdersAsync(int userId);
        Task<string> GenerateOrderCodeAsync();
        Task<int> GetOrderCountForUserInTimeRangeAsync(int userId, int days);
        Task UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}