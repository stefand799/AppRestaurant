using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppRestaurant.Models;
using AppRestaurant.ViewModels;

namespace AppRestaurant.ViewModels.Components
{
    public partial class FoodTileViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private decimal _price;

        [ObservableProperty]
        private string _imagePath;

        [ObservableProperty]
        private int _quantity = 1;

        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private bool _isAvailable = true;
        
        [ObservableProperty]
        private string _itemType; // "Dish" or "Meal"

        // Command to view food details
        [RelayCommand]
        private void ViewFoodDetails()
        {
            // This will be implemented in the parent ViewModel
            // The parent ViewModel will handle navigation to the food details page
        }
    }
}