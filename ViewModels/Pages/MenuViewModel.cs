using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppRestaurant.Models;
using AppRestaurant.Services.Menu;
using AppRestaurant.ViewModels.Components;

namespace AppRestaurant.ViewModels.Pages
{
    public partial class MenuPageViewModel : ViewModelBase
    {
        private readonly IMenuService _menuService;

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
        
        // Default constructor for design-time
        public MenuPageViewModel()
        {
            // In design time, just load some dummy data
            LoadDummyData();
        }
        
        // Constructor with dependencies for runtime
        public MenuPageViewModel(IMenuService menuService)
        {
            _menuService = menuService;
            
            // Load data asynchronously
            Task.Run(async () => await LoadDataAsync());
        }
        
        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                
                // Load categories
                var categoriesResult = await _menuService.GetAllCategoriesAsync();
                Categories.Clear();
                foreach (var category in categoriesResult)
                {
                    Categories.Add(category);
                }
                
                // Load allergens
                var allergensResult = await _menuService.GetAllAllergensAsync();
                Allergens.Clear();
                foreach (var allergen in allergensResult)
                {
                    Allergens.Add(allergen);
                }
                
                // Load dishes and meals
                await LoadMenuItemsAsync();
            }
            catch (Exception ex)
            {
                // In real app, handle this error (log it, show message, etc.)
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private async Task LoadMenuItemsAsync()
        {
            try
            {
                FoodItems.Clear();
                
                // Get all dishes
                var dishes = await _menuService.GetAllDishesAsync();
                
                // Filter if needed
                if (ShowOnlyAvailable)
                {
                    dishes = dishes.Where(d => d.Availability).ToList();
                }
                
                if (SelectedCategory != null)
                {
                    dishes = dishes.Where(d => d.CategoryId == SelectedCategory.Id).ToList();
                }
                
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    dishes = dishes.Where(d => d.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                
                if (AllergenFilterActive && SelectedAllergens.Any())
                {
                    var allergenIds = SelectedAllergens.Select(a => a.Id).ToList();
                    dishes = dishes.Where(d => 
                        !d.Allergens.Any(a => allergenIds.Contains(a.Id))).ToList();
                }
                
                // Map dishes to FoodTileViewModel
                foreach (var dish in dishes)
                {
                    FoodItems.Add(new FoodTileViewModel
                    {
                        Id = dish.Id,
                        Name = dish.Name,
                        Price = dish.Price,
                        ImagePath = dish.ImageUrl,
                        IsAvailable = dish.Availability,
                        ItemType = "Dish"
                    });
                }
                
                // Get all meals
                var meals = await _menuService.GetAllMealsAsync();
                
                // Filter if needed
                if (SelectedCategory != null)
                {
                    meals = meals.Where(m => m.CategoryId == SelectedCategory.Id).ToList();
                }
                
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    meals = meals.Where(m => m.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                
                // Check availability for each meal
                foreach (var meal in meals)
                {
                    bool isAvailable = await _menuService.CheckMealAvailabilityAsync(meal.Id);
                    
                    if (ShowOnlyAvailable && !isAvailable)
                    {
                        continue;
                    }
                    
                    // Calculate final price with discount
                    decimal finalPrice = await _menuService.CalculateMealPriceAsync(meal.Id);
                    
                    FoodItems.Add(new FoodTileViewModel
                    {
                        Id = meal.Id,
                        Name = meal.Name,
                        Price = finalPrice,
                        ImagePath = meal.ImageUrl,
                        IsAvailable = isAvailable,
                        ItemType = "Meal"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading menu items: {ex.Message}");
                
                // If we fail to load actual data, load dummy data for development
                if (_menuService == null)
                {
                    LoadDummyData();
                }
            }
        }
        
        private void LoadDummyData()
        {
            // Add some mock categories
            Categories.Clear();
            Categories.Add(new Category { Id = 1, Name = "Appetizers" });
            Categories.Add(new Category { Id = 2, Name = "Main Dishes" });
            Categories.Add(new Category { Id = 3, Name = "Sides" });
            Categories.Add(new Category { Id = 4, Name = "Desserts" });
            Categories.Add(new Category { Id = 5, Name = "Beverages" });
            
            // Add some mock allergens
            Allergens.Clear();
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
            
            FoodItems.Clear();
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
        private async Task Search()
        {
            IsLoading = true;
            
            try
            {
                await LoadMenuItemsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during search: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        [RelayCommand]
        private async Task ApplyFilters()
        {
            IsLoading = true;
            
            try
            {
                await LoadMenuItemsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying filters: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        [RelayCommand]
        private async Task ClearFilters()
        {
            SearchText = string.Empty;
            SelectedCategory = null;
            ShowOnlyAvailable = true;
            AllergenFilterActive = false;
            SelectedAllergens.Clear();
            
            IsLoading = true;
            
            try
            {
                await LoadMenuItemsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing filters: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        [RelayCommand]
        private async Task SelectCategory(Category category)
        {
            IsLoading = true;
            
            try
            {
                CurrentCategory = category;
                SelectedCategory = category;
                await LoadMenuItemsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting category: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        [RelayCommand]
        private async Task SelectAllCategories()
        {
            IsLoading = true;
            
            try
            {
                CurrentCategory = null;
                SelectedCategory = null;
                await LoadMenuItemsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting all categories: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
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