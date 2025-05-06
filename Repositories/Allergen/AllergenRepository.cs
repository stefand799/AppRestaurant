using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppRestaurant.Data;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.Allergen
{
    public class AllergenRepository : BaseRepository<Models.Allergen>, IAllergenRepository
    {
        public AllergenRepository(AppRestaurantDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Models.Allergen>> GetAllWithDishesAsync()
        {
            return await _context.Allergens
                .Include(a => a.Dishes)
                .ToListAsync();
        }

        public async Task<Models.Allergen> GetByIdWithDishesAsync(int id)
        {
            return await _context.Allergens
                .Include(a => a.Dishes)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Models.Allergen>> GetByDishIdAsync(int dishId)
        {
            return await _context.Allergens
                .Where(a => a.Dishes.Any(d => d.Id == dishId))
                .ToListAsync();
        }

        public async Task AddAllergenToDishAsync(int allergenId, int dishId)
        {
            var allergen = await _context.Allergens.FindAsync(allergenId);
            var dish = await _context.Dishes.FindAsync(dishId);
            
            if (allergen != null && dish != null)
            {
                if (dish.Allergens == null)
                {
                    dish.Allergens = new List<Models.Allergen>();
                }
                
                dish.Allergens.Add(allergen);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAllergenFromDishAsync(int allergenId, int dishId)
        {
            var allergen = await _context.Allergens.FindAsync(allergenId);
            var dish = await _context.Dishes
                .Include(d => d.Allergens)
                .FirstOrDefaultAsync(d => d.Id == dishId);
            
            if (allergen != null && dish != null && dish.Allergens != null)
            {
                dish.Allergens.Remove(allergen);
                await _context.SaveChangesAsync();
            }
        }
    }
}