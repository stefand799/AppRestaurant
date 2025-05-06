using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppRestaurant.Models;
using AppRestaurant.Services.Menu;
using AppRestaurant.Services.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace AppRestaurant.ViewModels.Pages
{
    public partial class ProductItemPageViewModel : ViewModelBase, IParametrizedViewModel
    {
        private readonly IMenuService _menuService;

        [ObservableProperty]
        private int _productId;
        
        [ObservableProperty]
        private string _name = "Loading...";
        
        [ObservableProperty]
        private string _description = "Loading product details...";
        
        [ObservableProperty]
        private decimal _price;
        
        [ObservableProperty]
        private ObservableCollection<string> _imagePaths = new ObservableCollection<string>();
        
        [ObservableProperty]
        private int _currentImageIndex = 0;
        
        [ObservableProperty]
        private string _currentImagePath;
        
        [ObservableProperty]
        private ObservableCollection<Allergen> _allergens = new ObservableCollection<Allergen>();
        
        [ObservableProperty]
        private bool _isAvailable = true;
        
        [ObservableProperty]
        private string _servingSize = "";
        
        [ObservableProperty]
        private string _servingUnit = "";
        
        [ObservableProperty]
        private string _itemType = ""; // "Dish" or "Meal"
        
        [ObservableProperty]
        private int _quantity = 1;
        
        [ObservableProperty]
        private bool _isLoading = true;
        
        public ProductItemPageViewModel()
        {
            // Try to get MenuService from IoC container if available
            _menuService = App.ServiceProvider?.GetService<IMenuService>();
            
            // If not available, show dummy data
            if (_menuService == null)
            {
                LoadDummyData();
                IsLoading = false;
            }
        }
        
        // IParametrizedViewModel implementation
        public void Initialize(object parameter)
        {
            if (parameter is int id)
            {
                ProductId = id;
                
                // If we have a menu service, load the actual data
                if (_menuService != null)
                {
                    _ = LoadProductDataAsync(id);
                }
                else
                {
                    // Otherwise, just show dummy data
                    LoadDummyData();
                    IsLoading = false;
                }
            }
        }
        
        private async Task LoadProductDataAsync(int id)
        {
            IsLoading = true;
            
            try
            {
                // First, try to load as a dish
                var dish = await _menuService.GetDishByIdAsync(id);
                
                if (dish != null)
                {
                    // It's a dish
                    ItemType = "Dish";
                    Name = dish.Name;
                    Price = dish.Price;
                    ServingSize = dish.ServingSize.ToString();
                    ServingUnit = dish.ServingUnit;
                    IsAvailable = dish.Availability;
                    Description = $"A delicious {dish.Name} from our {dish.Category?.Name?.ToLower() ?? "menu"}.";
                    
                    // Add the image
                    ImagePaths.Clear();
                    if (!string.IsNullOrEmpty(dish.ImageUrl))
                    {
                        ImagePaths.Add(dish.ImageUrl);
                        CurrentImagePath = dish.ImageUrl;
                    }
                    else
                    {
                        // Default image
                        ImagePaths.Add("/Assets/placeholder-food.png");
                        CurrentImagePath = "/Assets/placeholder-food.png";
                    }
                    
                    // Add allergens
                    Allergens.Clear();
                    foreach (var allergen in dish.Allergens)
                    {
                        Allergens.Add(allergen);
                    }
                }
                else
                {
                    // Try to load as a meal
                    var meal = await _menuService.GetMealByIdAsync(id);
                    
                    if (meal != null)
                    {
                        // It's a meal
                        ItemType = "Meal";
                        Name = meal.Name;
                        Price = await _menuService.CalculateMealPriceAsync(meal.Id);
                        IsAvailable = await _menuService.CheckMealAvailabilityAsync(meal.Id);
                        Description = $"A special meal combination from our {meal.Category?.Name?.ToLower() ?? "menu"}.";
                        
                        // Add the image
                        ImagePaths.Clear();
                        if (!string.IsNullOrEmpty(meal.ImageUrl))
                        {
                            ImagePaths.Add(meal.ImageUrl);
                            CurrentImagePath = meal.ImageUrl;
                        }
                        else
                        {
                            // Default image
                            ImagePaths.Add("/Assets/placeholder-food.png");
                            CurrentImagePath = "/Assets/placeholder-food.png";
                        }
                        
                        // Get the dishes in the meal for serving size display
                        var dishesInMeal = await _menuService.GetDishesByMealIdAsync(meal.Id);
                        if (dishesInMeal.Any())
                        {
                            var servingSizeText = string.Join(", ", 
                                dishesInMeal.Select(d => $"{d.Dish.Name} ({d.DishServingSize}{d.Dish.ServingUnit})"));
                            ServingSize = servingSizeText;
                        }
                        
                        // Collect allergens from all dishes in the meal
                        Allergens.Clear();
                        foreach (var dishInMeal in dishesInMeal)
                        {
                            foreach (var allergen in dishInMeal.Dish.Allergens)
                            {
                                if (!Allergens.Any(a => a.Id == allergen.Id))
                                {
                                    Allergens.Add(allergen);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Neither dish nor meal found
                        Name = "Item Not Found";
                        Description = "The requested item could not be found.";
                        IsAvailable = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading product data: {ex.Message}");
                Name = "Error Loading Item";
                Description = "An error occurred while loading the item details.";
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private void LoadDummyData()
        {
            // Load dummy data for the product
            Name = "Delicious Food Item";
            Description = "This delicious dish is prepared with the finest ingredients. Our chef's special recipe combines traditional flavors with a modern twist to create a unique culinary experience. Perfect for sharing or enjoying on your own.";
            Price = 12.99m;
            ServingSize = "300";
            ServingUnit = "g";
            ItemType = "Dish";
            IsAvailable = true;
            
            // Add dummy images if none are added yet
            ImagePaths.Clear();
            ImagePaths.Add("/Assets/placeholder-food.png");
            ImagePaths.Add("/Assets/placeholder-food.png");
            ImagePaths.Add("/Assets/placeholder-food.png");
            
            // Set the current image
            CurrentImagePath = ImagePaths[0];
            
            // Add dummy allergens
            Allergens.Clear();
            Allergens.Add(new Allergen { Id = 1, Name = "Gluten" });
            Allergens.Add(new Allergen { Id = 2, Name = "Dairy" });
        }
        
        [RelayCommand]
        private void NavigateBack()
        {
            // This should be handled by the parent view model
            // For now, we'll just log to the console
            Console.WriteLine("Navigate back requested");
            
            // Try to get the navigation service
            var navigationService = App.ServiceProvider?.GetService<INavigationService>();
            if (navigationService != null)
            {
                // The appropriate screen based on user type
                // Since we don't know which screen we came from, we'll just go to the login screen
                navigationService.ToLoginScreen();
            }
        }
        
        [RelayCommand]
        private void NextImage()
        {
            if (ImagePaths.Count > 0)
            {
                CurrentImageIndex = (CurrentImageIndex + 1) % ImagePaths.Count;
                CurrentImagePath = ImagePaths[CurrentImageIndex];
            }
        }
        
        [RelayCommand]
        private void PreviousImage()
        {
            if (ImagePaths.Count > 0)
            {
                CurrentImageIndex = (CurrentImageIndex - 1 + ImagePaths.Count) % ImagePaths.Count;
                CurrentImagePath = ImagePaths[CurrentImageIndex];
            }
        }
        
        [RelayCommand]
        private void AddToCart()
        {
            // This would be handled by a shopping cart service
            Console.WriteLine($"Adding {Quantity} {Name} to cart");
            
            // For now, just show a message - in a real app, you'd use a message service
            // or add to cart and navigate
        }
        
        [RelayCommand]
        private void IncreaseQuantity()
        {
            Quantity++;
        }
        
        [RelayCommand]
        private void DecreaseQuantity()
        {
            if (Quantity > 1)
            {
                Quantity--;
            }
        }
    }
}