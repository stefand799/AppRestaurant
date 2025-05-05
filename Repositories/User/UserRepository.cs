using System.Threading.Tasks;
using AppRestaurant.Data;
using AppRestaurant.Models;
using AppRestaurant.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AppRestaurant.Repositories.User
{
    public class UserRepository : Base.BaseRepository<Models.User>, IUserRepository
    {
        public UserRepository(AppRestaurantDbContext context) : base(context)
        {
        }
        
        public async Task<Models.User?> GetUserByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}