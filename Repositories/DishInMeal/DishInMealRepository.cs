using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppRestaurant.Data;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.DishInMeal
{
    public class DishInMealRepository : BaseRepository<Models.DishInMeal>, IDishInMealRepository
    {
        public DishInMealRepository(AppRestaurantDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Models.DishInMeal>> GetByMealIdAsync(int mealId)
        {
            return await _context.DishInMeals
                .Include(dm => dm.Dish)
                .Where(dm => dm.MealId == mealId)
                .ToListAsync();
        }

        public async Task<Models.DishInMeal> GetByIdWithDetailsAsync(int id)
        {
            return await _context.DishInMeals
                .Include(dm => dm.Dish)
                .Include(dm => dm.Meal)
                .FirstOrDefaultAsync(dm => dm.Id == id);
        }

        public async Task AddDishToMealAsync(int mealId, int dishId, decimal servingSize, decimal price)
        {
            var dishInMeal = new Models.DishInMeal
            {
                MealId = mealId,
                DishId = dishId,
                DishServingSize = servingSize,
                DishPrice = price
            };
            
            await _context.DishInMeals.AddAsync(dishInMeal);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveDishFromMealAsync(int mealId, int dishId)
        {
            var dishInMeal = await _context.DishInMeals
                .FirstOrDefaultAsync(dm => dm.MealId == mealId && dm.DishId == dishId);
            
            if (dishInMeal != null)
            {
                _context.DishInMeals.Remove(dishInMeal);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DishExistsInMealAsync(int mealId, int dishId)
        {
            return await _context.DishInMeals
                .AnyAsync(dm => dm.MealId == mealId && dm.DishId == dishId);
        }
    }
}