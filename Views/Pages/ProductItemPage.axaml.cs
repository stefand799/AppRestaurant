using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AppRestaurant.ViewModels.Pages;

namespace AppRestaurant.Views.Pages
{
    public partial class ProductItemPage : UserControl
    {
        public ProductItemPage()
        {
            InitializeComponent();
            
            // Initialize with an empty ViewModel if none is provided
            if (DataContext == null)
            {
                DataContext = new ProductItemPageViewModel();
            }
        }
    }
}