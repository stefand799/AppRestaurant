using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Order;
using AppRestaurant.Repositories.OrderItem;
using AppRestaurant.Repositories.Dish;
using AppRestaurant.Repositories.Meal;

namespace AppRestaurant.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IMealRepository _mealRepository;
        private readonly decimal _freeDeliveryThreshold;
        private readonly decimal _deliveryFee;
        private readonly int _loyaltyOrderCount;
        private readonly int _loyaltyTimeSpan;
        private readonly decimal _loyaltyDiscountPercentage;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IDishRepository dishRepository,
            IMealRepository mealRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _dishRepository = dishRepository;
            _mealRepository = mealRepository;
            
            // Load settings from AppConfiguration
            _freeDeliveryThreshold = AppConfiguration.FreeDeliveryThreshold;
            _deliveryFee = AppConfiguration.DeliveryFee;
            _loyaltyOrderCount = AppConfiguration.LoyaltyOrderCount;
            _loyaltyTimeSpan = AppConfiguration.LoyaltyTimeSpan;
            _loyaltyDiscountPercentage = AppConfiguration.LoyaltyDiscountPercentage;
        }

        public async Task<IEnumerable<Models.Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllWithDetailsAsync();
        }

        public async Task<Models.Order> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Models.Order>> GetUserOrdersAsync(int userId)
        {
            return await _orderRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Models.Order>> GetActiveOrdersAsync()
        {
            return await _orderRepository.GetActiveOrdersAsync();
        }

        public async Task<IEnumerable<Models.Order>> GetUserActiveOrdersAsync(int userId)
        {
            return await _orderRepository.GetUserActiveOrdersAsync(userId);
        }

        public async Task<Models.Order> CreateOrderAsync(Models.Order order, List<Models.OrderItem> orderItems)
        {
            // Generate a unique order code
            order.OrderCode = await _orderRepository.GenerateOrderCodeAsync();
            order.OrderDate = DateTime.Now;
            order.Status = "Registered";

            // Calculate total price before discounts
            decimal totalPrice = orderItems.Sum(item => item.TotalPrice);
            order.TotalPrice = totalPrice;

            // Apply discounts if eligible
            await ApplyDiscountsAsync(order);

            // Apply delivery fee if below threshold
            if (order.TotalPrice < _freeDeliveryThreshold)
            {
                order.TransportCost = _deliveryFee;
            }
            else
            {
                order.TransportCost = 0;
            }

            // Adjust final total price with transport cost
            order.TotalPrice += order.TransportCost;

            // Set estimated delivery time
            order.EstimatedDeliveryTime = DateTime.Now.AddMinutes(45); // Default 45 minutes

            // Save order
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveAsync();

            // Link order items to the order
            foreach (var item in orderItems)
            {
                item.OrderId = order.Id;
            }

            // Save order items
            await _orderItemRepository.AddRangeAsync(orderItems);
            await _orderItemRepository.SaveAsync();

            // Update stock quantities for dishes
            foreach (var item in orderItems.Where(i => i.Type == "Dish" && i.DishId.HasValue))
            {
                await _dishRepository.UpdateStockQuantityAsync(
                    item.DishId.Value, 
                    item.Qunatity // Typo in the model property name
                );
            }

            return order;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            try
            {
                await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            
            if (order == null || order.Status == "Delivered" || order.Status == "Cancelled")
            {
                return false;
            }
            
            // Update status to cancelled
            order.Status = "Cancelled";
            
            // Return items back to inventory if order was in preparation
            if (order.Status == "Preparing")
            {
                foreach (var item in order.OrderItems.Where(i => i.Type == "Dish" && i.DishId.HasValue))
                {
                    // Get the dish
                    var dish = await _dishRepository.GetByIdAsync(item.DishId.Value);
                    
                    if (dish != null)
                    {
                        // Increase the stock quantity
                        dish.StockQuantity += item.Qunatity;
                        
                        // Update availability
                        dish.Availability = true;
                        
                        // Save changes
                        await _dishRepository.UpdateAsync(dish);
                    }
                }
            }
            
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveAsync();
            
            return true;
        }

        private async Task ApplyDiscountsAsync(Models.Order order)
        {
            // Default - no discount
            order.DiscountPrecentage = 0;

            // Check if eligible for loyalty discount
            int orderCount = await _orderRepository.GetOrderCountForUserInTimeRangeAsync(
                order.UserId, 
                _loyaltyTimeSpan
            );
            
            if (orderCount >= _loyaltyOrderCount)
            {
                order.DiscountPrecentage = _loyaltyDiscountPercentage;
                order.TotalPrice = order.TotalPrice * (1 - (order.DiscountPrecentage / 100));
            }
        }
    }
}