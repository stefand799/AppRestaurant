using System.Threading.Tasks;
using AppRestaurant.Repositories.User;
using AppRestaurant.Services.CurrentUser;
using Microsoft.AspNetCore.Identity;

namespace AppRestaurant.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly PasswordHasher<Models.User> _passwordHasher;

        public AuthService(IUserRepository userRepository, ICurrentUserService currentUserService, PasswordHasher<Models.User> passwordHasher)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _passwordHasher = passwordHasher;
        }
        
        public async Task<Models.User?> Login(string email, string password)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return user;
        }
        
        public async Task<Models.User?> Register(Models.User user)
        {
            var existingUser = await _userRepository.GetUserByEmail(user.Email);
            if (existingUser != null)
                return null;

            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();

            return user;
        }
    }
}