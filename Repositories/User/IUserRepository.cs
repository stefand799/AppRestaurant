using System.Threading.Tasks;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Base;

namespace AppRestaurant.Repositories.User
{
    public interface IUserRepository : IBaseRepository<Models.User>
    {
        Task<Models.User?> GetUserByEmail(string email);
    }
}