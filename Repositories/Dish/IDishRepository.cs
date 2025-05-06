using System.Collections.Generic;
using System.Threading.Tasks;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.Dish
{
    public interface IDishRepository : IBaseRepository<Models.Dish>
    {
        Task<IEnumerable<Models.Dish>> GetAllWithCategoryAndAllergensAsync();
        Task<Models.Dish> GetByIdWithCategoryAndAllergensAsync(int id);
        Task<IEnumerable<Models.Dish>> GetByAvailabilityAsync(bool available);
        Task<IEnumerable<Models.Dish>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Models.Dish>> GetByNameContainsAsync(string searchTerm);
        Task<IEnumerable<Models.Dish>> GetLowStockAsync(decimal threshold);
        Task<IEnumerable<Models.Dish>> GetByAllergenAsync(int allergenId, bool exclude = false);
        Task<bool> UpdateStockQuantityAsync(int dishId, decimal quantityToReduce);
    }
}