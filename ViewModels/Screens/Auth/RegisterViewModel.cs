using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppRestaurant.Services.Auth;
using AppRestaurant.Services.CurrentUser;
using AppRestaurant.Services.Navigation;
using AppRestaurant.Models;

namespace AppRestaurant.ViewModels.Screen.Auth
{
    public partial class RegisterViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;

        public RegisterViewModel()
        {
            _navigationService = null!;
            _authService = null!;
            _currentUserService = null!;
        }

        public RegisterViewModel(INavigationService navigationService, IAuthService authService, ICurrentUserService currentUserService)
        {
            _navigationService = navigationService;
            _authService = authService;
            _currentUserService = currentUserService;
        }

        [ObservableProperty] private string _email = string.Empty;
        [ObservableProperty] private string _password = string.Empty;
        [ObservableProperty] private string _confirmPassword = string.Empty;
        [ObservableProperty] private string _firstName = string.Empty;
        [ObservableProperty] private string _lastName = string.Empty;
        [ObservableProperty] private string _phoneNumber = string.Empty;
        [ObservableProperty] private string _address = string.Empty;
        [ObservableProperty] private string _role = "Customer";
        [ObservableProperty] private string _errorMessage = string.Empty;

        [RelayCommand]
        public async Task RegisterCommand()
        {
            ErrorMessage = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(Password) ||
                    string.IsNullOrWhiteSpace(FirstName) ||
                    string.IsNullOrWhiteSpace(LastName) ||
                    string.IsNullOrWhiteSpace(Role))
                {
                    ErrorMessage = "All required fields must be filled.";
                    return;
                } 
                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Passwords do not match.";
                    return;
                }
                var user = await _authService.Register(new User
                {
                    Email = Email,
                    PasswordHash = Password,
                    FirstName = FirstName,
                    LastName = LastName,
                    PhoneNumber = PhoneNumber,
                    Address = Address,
                    Role = Role
                });

                if (user == null)
                {
                    ErrorMessage = "Registration failed. Email may already be in use.";
                    return;
                }

                _currentUserService.SetCurrentUser(user);

                if (user.Role.Equals("Employee", StringComparison.OrdinalIgnoreCase))
                {
                    _navigationService.ToEmployeeScreen();
                }
                else
                {
                    _navigationService.ToCustomerScreen();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during registration.";
                Console.WriteLine(ex);
            }
        }

        [RelayCommand]
        private void ToLoginScreenCommand()
        {
            _navigationService.ToLoginScreen();
        }
        
        [RelayCommand]
        private void ToGuestScreenCommand()
        {
            NavigationService.Instance.ToGuestScreen();
        }
    }
}
