using System;
using AppRestaurant.Models;

namespace AppRestaurant.Services.CurrentUser
{
    public class CurrentUserService : ICurrentUserService
    {
        private User _currentUser;
        public event EventHandler<User> UserChanged;

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                UserChanged?.Invoke(this, _currentUser);
            }
        }
        
        public bool IsLoggedIn => CurrentUser != null;
        
        public bool IsEmployee => CurrentUser?.Role == "Employee";
        
        public bool IsCustomer => CurrentUser?.Role == "Customer";
        
        public void LogOut()
        {
            CurrentUser = null;
        }
        
        public void SetCurrentUser(User user)
        {
            CurrentUser = user;
        }

    }
}