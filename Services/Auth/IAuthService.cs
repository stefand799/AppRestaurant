using System.Threading.Tasks;
using AppRestaurant.Models;

namespace AppRestaurant.Services.Auth
{
    public interface IAuthService
    {
        Task<Models.User?> Login(string email, string password);
        Task<Models.User?> Register(Models.User user);
    }
}