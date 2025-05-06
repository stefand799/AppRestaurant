using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AppRestaurant.ViewModels.Components;

namespace AppRestaurant.Views.Components
{
    public partial class FoodTileComponent : UserControl
    {
        public FoodTileComponent()
        {
            InitializeComponent();
            
            // Initialize with an empty ViewModel if none is provided
            if (DataContext == null)
            {
                DataContext = new FoodTileViewModel();
            }
        }
    }
}