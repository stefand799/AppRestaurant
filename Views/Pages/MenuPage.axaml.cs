using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AppRestaurant.ViewModels.Pages;

namespace AppRestaurant.Views.Pages
{
    public partial class MenuPage : UserControl
    {
        public MenuPage()
        {
            InitializeComponent();
            
            // Initialize with an empty ViewModel if none is provided
            if (DataContext == null)
            {
                DataContext = new MenuPageViewModel();
            }
        }
    }
}