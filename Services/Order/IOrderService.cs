using System.Collections.Generic;
using System.Threading.Tasks;
using AppRestaurant.Models;

namespace AppRestaurant.Services.Order
{
    public interface IOrderService
    {
        Task<IEnumerable<Models.Order>> GetAllOrdersAsync();
        Task<Models.Order> GetOrderByIdAsync(int id);
        Task<IEnumerable<Models.Order>> GetUserOrdersAsync(int userId);
        Task<IEnumerable<Models.Order>> GetActiveOrdersAsync();
        Task<IEnumerable<Models.Order>> GetUserActiveOrdersAsync(int userId);
        Task<Models.Order> CreateOrderAsync(Models.Order order, List<Models.OrderItem> orderItems);
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<bool> CancelOrderAsync(int orderId);
    }
}