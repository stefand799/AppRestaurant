using Avalonia.Controls;
using AppRestaurant.ViewModels;
using System;
namespace AppRestaurant.Services.Navigation
{
    public interface INavigationService
    {
        void Initialize(ContentControl mainContent);
        void Initialize(ContentControl mainContent, IServiceProvider serviceProvider);
        void NavigateTo<TViewModel>(object parameter = null) where TViewModel : ViewModelBase;
        void ToLoginScreen();
        void ToRegisterScreen();
        void ToGuestScreen();
        void ToCustomerScreen();
        void ToEmployeeScreen();
    }

    public interface IParametrizedViewModel
    {
        void Initialize(object parameter);
    }
}