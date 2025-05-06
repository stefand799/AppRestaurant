using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using AppRestaurant.Services.Navigation;
using AppRestaurant.Services.User;
using MenuPage = AppRestaurant.Views.Pages.MenuPage;

namespace AppRestaurant.ViewModels.Screens
{
    public partial class EmployeeViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private UserControl _currentScreen;

        public EmployeeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            
            // Set default screen - let's use MenuPage as placeholder
            CurrentScreen = new MenuPage();
        }

        [RelayCommand]
        private void LogOut()
        {
            // Get the CurrentUserService and clear the current user
            var currentUserService = App.ServiceProvider.GetService<CurrentUserService>();
            if (currentUserService != null)
            {
                currentUserService.LogOut(); // This method already exists in CurrentUserService
            }
    
            // Navigate back to the login page
            _navigationService.NavigateToLogin();
        }
        
        // Menu Management Screens
        [RelayCommand]
        private void NavigateToCategoriesScreen()
        {
            // For now, use MenuPage as placeholder
            CurrentScreen = new MenuPage();
        }
        
        [RelayCommand]
        private void NavigateToPreparationsScreen()
        {
            // For now, use MenuPage as placeholder
            CurrentScreen = new MenuPage();
        }
        
        [RelayCommand]
        private void NavigateToMenusScreen()
        {
            // For now, use MenuPage as placeholder
            CurrentScreen = new MenuPage();
        }
        
        [RelayCommand]
        private void NavigateToAllergensScreen()
        {
            // For now, use MenuPage as placeholder
            CurrentScreen = new MenuPage();
        }
        
        // Order Management Screens
        [RelayCommand]
        private void NavigateToAllOrdersScreen()
        {
            // For now, use MenuPage as placeholder
            CurrentScreen = new MenuPage();
        }
        
        [RelayCommand]
        private void NavigateToActiveOrdersScreen()
        {
            // For now, use MenuPage as placeholder
            CurrentScreen = new MenuPage();
        }
        
        // Inventory Management
        [RelayCommand]
        private void NavigateToLowStockScreen()
        {
            // For now, use MenuPage as placeholder
            CurrentScreen = new MenuPage();
        }
    }
}