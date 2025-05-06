using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppRestaurant.Services.Navigation;
using AppRestaurant.ViewModels.Pages;
using AppRestaurant.Views.Pages;
using System;

namespace AppRestaurant.ViewModels.Screens
{
    public partial class GuestViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private UserControl _currentScreen;

        // Default constructor for design-time
        public GuestViewModel()
        {
            // Use the singleton instance of NavigationService
            _navigationService = NavigationService.Instance;
            
            // Set default screen with its ViewModel
            var menuPage = new MenuPage();
            menuPage.DataContext = new MenuPageViewModel();
            CurrentScreen = menuPage;
        }

        // Constructor with navigation service for runtime
        public GuestViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            
            // Set default screen with its ViewModel
            var menuPage = new MenuPage();
            menuPage.DataContext = new MenuPageViewModel();
            CurrentScreen = menuPage;
        }

        [RelayCommand]
        public void NavigateToLogin()
        {
            _navigationService.NavigateToLogin();
        }

        [RelayCommand]
        private void NavigateToRegister()
        {
            _navigationService.NavigateToRegister();
        }
        
        [RelayCommand]
        private void NavigateToMenuScreen()
        {
            var menuPage = new MenuPage();
            menuPage.DataContext = new MenuPageViewModel();
            CurrentScreen = menuPage;
        }
        
        [RelayCommand]
        private void NavigateToFoodItemScreen(int foodItemId)
        {
            var productItemPage = new ProductItemPage();
            // Pass the ID to the product item page ViewModel
            // In a real app, you would load the actual product data here
            Console.WriteLine($"Navigating to food item with ID: {foodItemId}");
            
            // TODO: Create a ProductItemPageViewModel and load data for the foodItemId
            
            CurrentScreen = productItemPage;
        }
        
        // This method will be called by the FoodTile's ViewFoodDetailsCommand
        // and propagated up to this ViewModel
        [RelayCommand]
        private void ViewFoodDetails(int foodItemId)
        {
            // For guests, we could either:
            // 1. Navigate to the food item screen
            NavigateToFoodItemScreen(foodItemId);
            
            // Or 2. Prompt them to log in first (if you want to restrict detailed views)
            // _navigationService.NavigateToLogin();
        }
    }
}