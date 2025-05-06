using System.Collections.Generic;
using System.Threading.Tasks;
using AppRestaurant.Models;

namespace AppRestaurant.Services.Menu
{
    public interface IMenuService
    {
        // Category operations
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> GetCategoryWithItemsAsync(int id);
        Task<Category> AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);

        // Dish operations
        Task<IEnumerable<Dish>> GetAllDishesAsync();
        Task<Dish> GetDishByIdAsync(int id);
        Task<IEnumerable<Dish>> GetDishesByKeywordAsync(string keyword);
        Task<IEnumerable<Dish>> GetDishesByCategoryAsync(int categoryId);
        Task<IEnumerable<Dish>> GetAvailableDishesAsync();
        Task<IEnumerable<Dish>> GetLowStockDishesAsync(decimal threshold);
        Task<IEnumerable<Dish>> GetDishesByAllergenAsync(int allergenId, bool exclude);
        Task<Dish> AddDishAsync(Dish dish);
        Task UpdateDishAsync(Dish dish);
        Task DeleteDishAsync(int id);
        Task<bool> UpdateDishStockAsync(int dishId, decimal quantity);

        // Meal operations
        Task<IEnumerable<Meal>> GetAllMealsAsync();
        Task<Meal> GetMealByIdAsync(int id);
        Task<IEnumerable<Meal>> GetMealsByKeywordAsync(string keyword);
        Task<IEnumerable<Meal>> GetMealsByCategoryAsync(int categoryId);
        Task<bool> CheckMealAvailabilityAsync(int mealId);
        Task<decimal> CalculateMealPriceAsync(int mealId);
        Task<Meal> AddMealAsync(Meal meal);
        Task UpdateMealAsync(Meal meal);
        Task DeleteMealAsync(int id);

        // Allergen operations
        Task<IEnumerable<Allergen>> GetAllAllergensAsync();
        Task<Allergen> GetAllergenByIdAsync(int id);
        Task<IEnumerable<Allergen>> GetAllergensByDishIdAsync(int dishId);
        Task<Allergen> AddAllergenAsync(Allergen allergen);
        Task UpdateAllergenAsync(Allergen allergen);
        Task DeleteAllergenAsync(int id);
        Task AddAllergenToDishAsync(int allergenId, int dishId);
        Task RemoveAllergenFromDishAsync(int allergenId, int dishId);

        // DishInMeal operations
        Task<IEnumerable<DishInMeal>> GetDishesByMealIdAsync(int mealId);
        Task AddDishToMealAsync(int mealId, int dishId, decimal servingSize, decimal price);
        Task RemoveDishFromMealAsync(int mealId, int dishId);
        
        Task<IEnumerable<Dish>> SearchDishesAsync(
            string searchTerm = null, 
            int? categoryId = null, 
            bool onlyAvailable = false,
            IEnumerable<int> excludeAllergenIds = null);
            
        Task<IEnumerable<Meal>> SearchMealsAsync(
            string searchTerm = null, 
            int? categoryId = null,
            bool onlyAvailable = false,
            IEnumerable<int> excludeAllergenIds = null);
    }
}