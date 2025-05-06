using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppRestaurant.Data;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.Meal
{
    public class MealRepository : BaseRepository<Models.Meal>, IMealRepository
    {
        public MealRepository(AppRestaurantDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Models.Meal>> GetAllWithDetailsAsync()
        {
            return await _context.Meals
                .Include(m => m.Category)
                .Include(m => m.DishInMeals)
                    .ThenInclude(dm => dm.Dish)
                        .ThenInclude(d => d.Allergens)
                .ToListAsync();
        }

        public async Task<Models.Meal> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Meals
                .Include(m => m.Category)
                .Include(m => m.DishInMeals)
                    .ThenInclude(dm => dm.Dish)
                        .ThenInclude(d => d.Allergens)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Models.Meal>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Meals
                .Include(m => m.Category)
                .Include(m => m.DishInMeals)
                    .ThenInclude(dm => dm.Dish)
                .Where(m => m.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Meal>> GetByNameContainsAsync(string searchTerm)
        {
            return await _context.Meals
                .Include(m => m.Category)
                .Include(m => m.DishInMeals)
                    .ThenInclude(dm => dm.Dish)
                .Where(m => m.Name.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<bool> CheckAvailabilityAsync(int mealId)
        {
            var meal = await _context.Meals
                .Include(m => m.DishInMeals)
                    .ThenInclude(dm => dm.Dish)
                .FirstOrDefaultAsync(m => m.Id == mealId);

            if (meal == null)
                return false;

            // Check if all dishes in the meal are available
            return meal.DishInMeals.All(dm => dm.Dish.Availability);
        }
        
        public async Task<decimal> CalculateMealPriceAsync(int mealId, decimal discountPercentage)
        {
            var meal = await _context.Meals
                .Include(m => m.DishInMeals)
                .FirstOrDefaultAsync(m => m.Id == mealId);

            if (meal == null)
                return 0;

            decimal totalPrice = meal.DishInMeals.Sum(dm => dm.DishPrice);
            return totalPrice * (1 - (discountPercentage / 100));
        }
    }
}