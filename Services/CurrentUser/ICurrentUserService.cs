using AppRestaurant.Models;

namespace AppRestaurant.Services.CurrentUser
{
    public interface ICurrentUserService
    {
        void SetCurrentUser(Models.User user);
        void LogOut();
    }
}