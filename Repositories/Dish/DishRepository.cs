using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppRestaurant.Data;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.Dish
{
    public class DishRepository : BaseRepository<Models.Dish>, IDishRepository
    {
        public DishRepository(AppRestaurantDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Models.Dish>> GetAllWithCategoryAndAllergensAsync()
        {
            return await _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Allergens)
                .ToListAsync();
        }

        public async Task<Models.Dish> GetByIdWithCategoryAndAllergensAsync(int id)
        {
            return await _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Allergens)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Models.Dish>> GetByAvailabilityAsync(bool available)
        {
            return await _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Allergens)
                .Where(d => d.Availability == available)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Dish>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Allergens)
                .Where(d => d.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Dish>> GetByNameContainsAsync(string searchTerm)
        {
            return await _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Allergens)
                .Where(d => d.Name.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Dish>> GetLowStockAsync(decimal threshold)
        {
            return await _context.Dishes
                .Include(d => d.Category)
                .Where(d => d.StockQuantity <= threshold)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.Dish>> GetByAllergenAsync(int allergenId, bool exclude = false)
        {
            if (exclude)
            {
                return await _context.Dishes
                    .Include(d => d.Category)
                    .Include(d => d.Allergens)
                    .Where(d => !d.Allergens.Any(a => a.Id == allergenId))
                    .ToListAsync();
            }
            else
            {
                return await _context.Dishes
                    .Include(d => d.Category)
                    .Include(d => d.Allergens)
                    .Where(d => d.Allergens.Any(a => a.Id == allergenId))
                    .ToListAsync();
            }
        }

        public async Task<bool> UpdateStockQuantityAsync(int dishId, decimal quantityToReduce)
        {
            var dish = await _dbSet.FindAsync(dishId);
            if (dish == null || dish.StockQuantity < quantityToReduce)
                return false;

            dish.StockQuantity -= quantityToReduce;
            
            // Update availability if stock reaches 0
            if (dish.StockQuantity <= 0)
            {
                dish.Availability = false;
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}