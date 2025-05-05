using AppRestaurant.Models;

namespace AppRestaurant.Services.CurrentUser
{
    public interface ICurrentUserService
    {
        void SetCurrentUser(User user);
        void LogOut();
    }
}