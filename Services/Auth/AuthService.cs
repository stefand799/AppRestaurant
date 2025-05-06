using System.Threading.Tasks;
using AppRestaurant.Data;
using AppRestaurant.Repositories.User;
using CurrentUserService = AppRestaurant.Services.CurrentUser.CurrentUserService;
using Microsoft.AspNetCore.Identity;

namespace AppRestaurant.Services.Auth
{

    public class AuthService : IAuthService
    {
        private readonly AppRestaurantDbContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly CurrentUserService _currentUserService;

        public AuthService(AppRestaurantDbContext dbContext, IUserRepository userRepository, CurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }
        
        public async Task<Models.User?> Login(string email, string password)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
                return null;

            var passwordHasher = new PasswordHasher<Models.User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return user;
        }
        
        public async Task<Models.User?> Register(Models.User user)
        {
            var existingUser = await _userRepository.GetUserByEmail(user.Email);
            if (existingUser != null)
                return null;

            var passwordHasher = new PasswordHasher<Models.User>();
            user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordHash);

            await _userRepository.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

    }
}