using System.Collections.Generic;
using System.Threading.Tasks;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.Allergen
{
    public interface IAllergenRepository : IBaseRepository<Models.Allergen>
    {
        Task<IEnumerable<Models.Allergen>> GetAllWithDishesAsync();
        Task<Models.Allergen> GetByIdWithDishesAsync(int id);
        Task<IEnumerable<Models.Allergen>> GetByDishIdAsync(int dishId);
        Task AddAllergenToDishAsync(int allergenId, int dishId);
        Task RemoveAllergenFromDishAsync(int allergenId, int dishId);
    }
}