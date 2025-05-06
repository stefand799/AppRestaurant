using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppRestaurant.Models;
using AppRestaurant.ViewModels.Components;
using System.Linq;
using System;
using AppRestaurant.ViewModels;

namespace AppRestaurant.ViewModels.Pages
{
    public partial class MenuPageViewModel : ViewModelBase
    {
        // Properties for search and filtering
        [ObservableProperty]
        private string _searchText = string.Empty;
        
        [ObservableProperty]
        private Category _selectedCategory;
        
        [ObservableProperty]
        private bool _showOnlyAvailable = true;
        
        [ObservableProperty]
        private bool _allergenFilterActive = false;
        
        [ObservableProperty]
        private ObservableCollection<Allergen> _selectedAllergens = new ObservableCollection<Allergen>();
        
        // Collections for menu items
        [ObservableProperty]
        private ObservableCollection<FoodTileViewModel> _foodItems = new ObservableCollection<FoodTileViewModel>();
        
        [ObservableProperty]
        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();
        
        [ObservableProperty]
        private ObservableCollection<Allergen> _allergens = new ObservableCollection<Allergen>();
        
        // Selected category for filtering
        [ObservableProperty]
        private Category _currentCategory;
        
        // Is data loading indicator
        [ObservableProperty]
        private bool _isLoading = false;
        
        public MenuPageViewModel()
        {
            // In a real application, these would be loaded from a service
            // For now, we'll just create some dummy data
            LoadDummyData();
        }
        
        private void LoadDummyData()
        {
            // Add some mock categories
            Categories.Add(new Category { Id = 1, Name = "Appetizers" });
            Categories.Add(new Category { Id = 2, Name = "Main Dishes" });
            Categories.Add(new Category { Id = 3, Name = "Sides" });
            Categories.Add(new Category { Id = 4, Name = "Desserts" });
            Categories.Add(new Category { Id = 5, Name = "Beverages" });
            
            // Add some mock allergens
            Allergens.Add(new Allergen { Id = 1, Name = "Gluten" });
            Allergens.Add(new Allergen { Id = 2, Name = "Dairy" });
            Allergens.Add(new Allergen { Id = 3, Name = "Nuts" });
            Allergens.Add(new Allergen { Id = 4, Name = "Shellfish" });
            
            // Add some mock food items
            string[] dishNames = {
                "Grilled Salmon", "Caesar Salad", "Margherita Pizza", "Beef Burger", 
                "Vegetable Pasta", "Chicken Wings", "Mushroom Risotto", "Fish Tacos",
                "Greek Salad", "Steak Frites", "Sushi Platter", "Vegetable Curry"
            };
            
            Random random = new Random();
            
            for (int i = 0; i < 12; i++)
            {
                FoodItems.Add(new FoodTileViewModel
                {
                    Id = i + 1,
                    Name = dishNames[i % dishNames.Length],
                    Price = Math.Round(random.Next(599, 2499) / 100.0m, 2),
                    Quantity = 1,
                    IsAvailable = random.Next(10) > 1, // 90% chance of being available
                    ItemType = random.Next(2) == 0 ? "Dish" : "Meal"
                });
            }
        }
        
        [RelayCommand]
        private void Search()
        {
            IsLoading = true;
            
            // In a real application, this would filter the items based on search criteria
            LoadDummyData(); // Reload dummy data
            
            // Apply search filter if search text is not empty
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var filteredItems = FoodItems.Where(item => 
                    item.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                
                FoodItems.Clear();
                foreach (var item in filteredItems)
                {
                    FoodItems.Add(item);
                }
            }
            
            IsLoading = false;
        }
        
        [RelayCommand]
        private void ApplyFilters()
        {
            IsLoading = true;
            
            // In a real application, this would apply selected filters
            Search();
            
            // Apply availability filter if selected
            if (ShowOnlyAvailable)
            {
                var availableItems = FoodItems.Where(item => item.IsAvailable).ToList();
                FoodItems.Clear();
                foreach (var item in availableItems)
                {
                    FoodItems.Add(item);
                }
            }
            
            IsLoading = false;
        }
        
        [RelayCommand]
        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedCategory = null;
            ShowOnlyAvailable = true;
            AllergenFilterActive = false;
            SelectedAllergens.Clear();
            
            // Reload all items
            LoadDummyData();
        }
        
        [RelayCommand]
        private void SelectCategory(Category category)
        {
            IsLoading = true;
            
            CurrentCategory = category;
            
            // Reload dummy data
            LoadDummyData();
            
            // Apply category filter
            if (category != null)
            {
                // In a real app, this would filter by actual category
                // For demo, we'll just take a subset based on category ID
                int itemCount = category.Id switch
                {
                    1 => 4,  // Appetizers
                    2 => 6,  // Main Dishes
                    3 => 3,  // Sides
                    4 => 2,  // Desserts
                    5 => 3,  // Beverages
                    _ => 12  // All
                };
                
                while (FoodItems.Count > itemCount)
                {
                    FoodItems.RemoveAt(FoodItems.Count - 1);
                }
            }
            
            IsLoading = false;
        }
        
        [RelayCommand]
        private void SelectAllCategories()
        {
            CurrentCategory = null;
            LoadDummyData(); // Show all items
        }
        
        [RelayCommand]
        private void ViewFoodDetails(int foodItemId)
        {
            // This would navigate to the food details page in a real app
            // For now, we'll just print to the console for debugging
            Console.WriteLine($"Viewing food item with ID: {foodItemId}");
            
            // In the customer/guest view models, this would be implemented to handle navigation
        }
    }
}