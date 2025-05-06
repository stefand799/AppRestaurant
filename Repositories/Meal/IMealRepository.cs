using System.Collections.Generic;
using System.Threading.Tasks;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.Meal
{
    public interface IMealRepository : IBaseRepository<Models.Meal>
    {
        Task<IEnumerable<Models.Meal>> GetAllWithDetailsAsync();
        Task<Models.Meal> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Models.Meal>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Models.Meal>> GetByNameContainsAsync(string searchTerm);
        Task<bool> CheckAvailabilityAsync(int mealId);
        Task<decimal> CalculateMealPriceAsync(int mealId, decimal discountPercentage);
    }
}