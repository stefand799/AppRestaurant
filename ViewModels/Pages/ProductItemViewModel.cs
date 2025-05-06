using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppRestaurant.Models;
using System;
using System.Linq;
using AppRestaurant.ViewModels;

namespace AppRestaurant.ViewModels.Pages
{
    public partial class ProductItemPageViewModel : ViewModelBase, Services.Navigation.IParametrizedViewModel
    {
        [ObservableProperty]
        private int _productId;
        
        [ObservableProperty]
        private string _name = "Delicious Food Item";
        
        [ObservableProperty]
        private string _description = "This delicious dish is prepared with the finest ingredients. Our chef's special recipe combines traditional flavors with a modern twist to create a unique culinary experience. Perfect for sharing or enjoying on your own.";
        
        [ObservableProperty]
        private decimal _price = 12.99m;
        
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
        private string _servingSize = "300";
        
        [ObservableProperty]
        private string _servingUnit = "g";
        
        [ObservableProperty]
        private string _itemType = "Dish"; // "Dish" or "Meal"
        
        [ObservableProperty]
        private int _quantity = 1;
        
        public ProductItemPageViewModel()
        {
            // Default constructor loads dummy data
            LoadDummyData();
        }
        
        public void Initialize(object parameter)
        {
            if (parameter is int id)
            {
                ProductId = id;
                // In a real app, this would load the product data from a service
                LoadDummyData();
            }
        }
        
        private void LoadDummyData()
        {
            // Load dummy data for the product (already set in default properties above)
            // Add dummy images if none are added yet
            if (ImagePaths.Count == 0)
            {
                ImagePaths.Add("/Assets/avalonia-logo.ico");
                ImagePaths.Add("/Assets/avalonia-logo.ico");
                ImagePaths.Add("/Assets/avalonia-logo.ico");
            }
            
            // Set the current image if not already set
            if (string.IsNullOrEmpty(CurrentImagePath) && ImagePaths.Count > 0)
            {
                CurrentImagePath = ImagePaths[0];
            }
            
            // Add dummy allergens if none are added yet
            if (Allergens.Count == 0)
            {
                Allergens.Add(new Allergen { Id = 1, Name = "Gluten" });
                Allergens.Add(new Allergen { Id = 2, Name = "Dairy" });
                Allergens.Add(new Allergen { Id = 3, Name = "Nuts" });
            }
        }
        
        [RelayCommand]
        private void NavigateBack()
        {
            // This would be handled by the parent ViewModel
            // For now, we'll just log to the console
            Console.WriteLine("Navigate back to menu");
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
            // For now, we'll just log to the console
            Console.WriteLine($"Adding {Quantity} {Name} to cart");
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