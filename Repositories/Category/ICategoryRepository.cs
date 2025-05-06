using System.Collections.Generic;
using System.Threading.Tasks;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.Category
{
    public interface ICategoryRepository : IBaseRepository<Models.Category>
    {
        Task<IEnumerable<Models.Category>> GetAllWithDishesAndMealsAsync();
        Task<Models.Category> GetByIdWithDishesAndMealsAsync(int id);
    }
}