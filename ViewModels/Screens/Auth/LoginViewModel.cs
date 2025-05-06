using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppRestaurant.Services.Auth;
using AppRestaurant.Services.CurrentUser;
using AppRestaurant.Services.Navigation;

namespace AppRestaurant.ViewModels.Screen.Auth
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;   
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;

        public LoginViewModel()
        {
            _navigationService = NavigationService.Instance;
            _authService = null;
            _currentUserService = null;
        }

        public LoginViewModel(INavigationService navigationService, IAuthService authService, ICurrentUserService currentUserService)
        {
            _navigationService = navigationService;
            _authService = authService;
            _currentUserService = currentUserService;
        }
        
        [ObservableProperty]
        private string _email = string.Empty;
        
        [ObservableProperty]
        private string _password = string.Empty;
        
        [ObservableProperty]
        private string _errorMessage = string.Empty;

        // Change this from RelayCommand to IRelayCommand and make it public
        [RelayCommand]
        public async Task Login()
        {
            ErrorMessage = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
                {
                    ErrorMessage = "Email and password are required.";
                    return;
                }

                if (_authService == null)
                {
                    // For development/testing
                    NavigationService.Instance.ToGuestScreen();
                    return;
                }

                var user = await _authService.Login(Email, Password);

                if (user == null)
                {
                    ErrorMessage = "Invalid login credentials.";
                    return;
                }

                if (user.Role.Equals("Employee", StringComparison.OrdinalIgnoreCase))
                {
                    NavigationService.Instance.ToEmployeeScreen();
                }
                else
                {
                    NavigationService.Instance.ToCustomerScreen();
                }
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Something went wrong.";
                Console.WriteLine(ex);
            }
        }

        [RelayCommand]
        public void ToRegisterScreen()
        {
            NavigationService.Instance.ToRegisterScreen();
        }

        [RelayCommand]
        public void ToGuestScreen()
        {
            NavigationService.Instance.ToGuestScreen();
        }
    }
}