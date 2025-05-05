using System.Threading.Tasks;
using AppRestaurant.Models;

namespace AppRestaurant.Services.Auth
{
    public interface IAuthService
    {
        Task<User?> Login(string email, string password);
        Task<User?> Register(User user);
    }
}