using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppRestaurant.Data;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.Category
{
    public class CategoryRepository : BaseRepository<Models.Category>, ICategoryRepository
    {
        public CategoryRepository(AppRestaurantDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Models.Category>> GetAllWithDishesAndMealsAsync()
        {
            return await _context.Categories
                .Include(c => c.Dishes)
                .Include(c => c.Meals)
                .ToListAsync();
        }

        public async Task<Models.Category> GetByIdWithDishesAndMealsAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Dishes)
                .Include(c => c.Meals)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}