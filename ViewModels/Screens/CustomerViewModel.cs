using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppRestaurant.Services.Navigation;
using AppRestaurant.ViewModels.Components;
using AppRestaurant.ViewModels.Pages;
using AppRestaurant.Views.Pages;
using System;
using AppRestaurant.Services.CurrentUser;
using AppRestaurant.Services.User;

namespace AppRestaurant.ViewModels.Screens
{
    public partial class CustomerViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private UserControl _currentScreen;

        // Default constructor for design-time
        public CustomerViewModel()
        {
            // Use the singleton instance of NavigationService
            _navigationService = NavigationService.Instance;
            
            // Set default screen with its ViewModel
            var menuPage = new MenuPage();
            menuPage.DataContext = new MenuViewModel();
            CurrentScreen = menuPage;
        }

        // Constructor with navigation service for runtime
        public CustomerViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            
            // Set default screen with its ViewModel
            var menuPage = new MenuPage();
            menuPage.DataContext = new MenuViewModel();
            CurrentScreen = menuPage;
        }

        
        
// In CustomerViewModel.cs
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
            _navigationService.ToLoginScreen();
        }
        
        [RelayCommand]
        private void NavigateToMenuScreen()
        {
            var menuPage = new MenuPage();
            menuPage.DataContext = new MenuViewModel();
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
        
        [RelayCommand]
        private void NavigateToOrdersScreen()
        {
            // For now, we'll use MenuPage as a placeholder
            // Later you can replace this with an OrdersPage
            NavigateToMenuScreen();
        }
        
        [RelayCommand]
        private void NavigateToProfileScreen()
        {
            // For now, we'll use MenuPage as a placeholder
            // Later you can replace this with a ProfilePage
            NavigateToMenuScreen();
        }
        
        [RelayCommand]
        private void NavigateToCartScreen()
        {
            // For now, we'll use MenuPage as a placeholder
            // Later you can replace this with a CartPage
            NavigateToMenuScreen();
        }
        
        // This method will be called by the FoodTile's ViewFoodDetailsCommand
        // and propagated up to this ViewModel
        [RelayCommand]
        public void ViewFoodDetails(int foodItemId)
        {
            // Create the product item page
            var productItemPage = new ProductItemPage();
            
            // Create and initialize the view model with the food item ID
            var viewModel = new ProductItemPageViewModel();
            viewModel.Initialize(foodItemId); // This calls the IParametrizedViewModel.Initialize method
            
            // Set the data context and navigate
            productItemPage.DataContext = viewModel;
            CurrentScreen = productItemPage;
            
            Console.WriteLine($"Navigating to food item details for ID: {foodItemId}");
        }
        
    }
}