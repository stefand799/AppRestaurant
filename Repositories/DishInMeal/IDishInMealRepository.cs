using System.Collections.Generic;
using System.Threading.Tasks;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.DishInMeal
{
    public interface IDishInMealRepository : IBaseRepository<Models.DishInMeal>
    {
        Task<IEnumerable<Models.DishInMeal>> GetByMealIdAsync(int mealId);
        Task<Models.DishInMeal> GetByIdWithDetailsAsync(int id);
        Task AddDishToMealAsync(int mealId, int dishId, decimal servingSize, decimal price);
        Task RemoveDishFromMealAsync(int mealId, int dishId);
        Task<bool> DishExistsInMealAsync(int mealId, int dishId);
    }
}