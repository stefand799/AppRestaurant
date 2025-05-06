using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppRestaurant.Models;
using AppRestaurant.Services.Menu;
using AppRestaurant.ViewModels.Components;
using Microsoft.Extensions.DependencyInjection;

namespace AppRestaurant.ViewModels.Pages
{
    public partial class MenuViewModel : ViewModelBase
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
        
        // Add this property for binding to UI
        [ObservableProperty]
        private string _allergenSelectionSummary = "None selected";
        
        // Collections for menu items
        [ObservableProperty]
        private ObservableCollection<FoodTileViewModel> _foodItems = new ObservableCollection<FoodTileViewModel>();
        
        [ObservableProperty]
        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();
        
        [ObservableProperty]
        private ObservableCollection<Allergen> _allergens = new ObservableCollection<Allergen>();
        
        // Is data loading indicator
        [ObservableProperty]
        private bool _isLoading = false;
        
        // Default constructor for design-time
        public MenuViewModel()
        {
            // Try to get MenuService from IoC container if available
            _menuService = App.ServiceProvider?.GetService<IMenuService>();
            
            // If not available (design time), just load dummy data
            if (_menuService == null)
            {
                LoadDummyData();
            }
            else
            {
                // Load data asynchronously
                _ = LoadDataAsync();
            }
            
            // Set up property changed handler to update the summary
            PropertyChanged += (sender, e) => {
                if (e.PropertyName == nameof(SelectedAllergens))
                {
                    UpdateAllergenSelectionSummary();
                }
            };
        }
        
        // Constructor with dependencies for runtime
        public MenuViewModel(IMenuService menuService)
        {
            _menuService = menuService;
            
            // Load data asynchronously
            _ = LoadDataAsync();
            
            // Set up property changed handler to update the summary
            PropertyChanged += (sender, e) => {
                if (e.PropertyName == nameof(SelectedAllergens))
                {
                    UpdateAllergenSelectionSummary();
                }
            };
        }
        
        private void UpdateAllergenSelectionSummary()
        {
            if (SelectedAllergens.Count == 0)
            {
                AllergenSelectionSummary = "None selected";
            }
            else if (SelectedAllergens.Count <= 2)
            {
                AllergenSelectionSummary = string.Join(", ", SelectedAllergens.Select(a => a.Name));
            }
            else
            {
                AllergenSelectionSummary = $"{SelectedAllergens.Count} allergens selected";
            }
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
                
                // Load menu items with current filters
                await LoadMenuItemsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
                
                // If we fail to load actual data, load dummy data for development
                if (_menuService == null)
                {
                    LoadDummyData();
                }
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
                IsLoading = true;
                FoodItems.Clear();
                
                // Get the excluded allergen IDs
                var excludeAllergenIds = AllergenFilterActive && SelectedAllergens.Any() 
                    ? SelectedAllergens.Select(a => a.Id).ToList() 
                    : null;
                
                // Get dishes with filters
                var dishes = await _menuService.SearchDishesAsync(
                    SearchText, 
                    SelectedCategory?.Id, 
                    ShowOnlyAvailable,
                    excludeAllergenIds);
                
                // Map dishes to FoodTileViewModel
                foreach (var dish in dishes)
                {
                    FoodItems.Add(new FoodTileViewModel
                    {
                        Id = dish.Id,
                        Name = dish.Name,
                        Price = dish.Price,
                        ImagePath = dish.ImageUrl ?? "/Assets/placeholder-food.png",
                        IsAvailable = dish.Availability,
                        ItemType = "Dish"
                    });
                }
                
                // Get meals with filters
                var meals = await _menuService.SearchMealsAsync(
                    SearchText, 
                    SelectedCategory?.Id, 
                    ShowOnlyAvailable,
                    excludeAllergenIds);
                
                // Map meals to FoodTileViewModel
                foreach (var meal in meals)
                {
                    // Calculate price with discount
                    decimal finalPrice = await _menuService.CalculateMealPriceAsync(meal.Id);
                    
                    // Check availability
                    bool isAvailable = await _menuService.CheckMealAvailabilityAsync(meal.Id);
                    
                    FoodItems.Add(new FoodTileViewModel
                    {
                        Id = meal.Id,
                        Name = meal.Name,
                        Price = finalPrice,
                        ImagePath = meal.ImageUrl ?? "/Assets/placeholder-food.png",
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
            finally
            {
                IsLoading = false;
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
                    ItemType = random.Next(2) == 0 ? "Dish" : "Meal",
                    ImagePath = "/Assets/placeholder-food.png"
                });
            }
        }
        
        [RelayCommand]
        private async Task Search()
        {
            try
            {
                IsLoading = true;
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
            try
            {
                IsLoading = true;
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
            
            try
            {
                IsLoading = true;
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
        public void ToggleAllergen(Allergen allergen)
        {
            if (allergen == null) return;
            
            // Check if the allergen is already selected
            var existingAllergen = SelectedAllergens.FirstOrDefault(a => a.Id == allergen.Id);
            
            if (existingAllergen != null)
            {
                // If found, remove it
                SelectedAllergens.Remove(existingAllergen);
            }
            else
            {
                // If not found, add it
                SelectedAllergens.Add(allergen);
            }
            
            // Update the summary
            UpdateAllergenSelectionSummary();
        }
        
        [RelayCommand]
        private void ViewFoodDetails(int foodItemId)
        {
            // This would navigate to the food details page in a real app
            Console.WriteLine($"Viewing food item with ID: {foodItemId}");
            
            // This will be implemented in the parent view model (GuestViewModel or CustomerViewModel)
            // to handle navigation to the product details page
        }
    }
}