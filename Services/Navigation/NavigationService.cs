using Avalonia.Controls;
using AppRestaurant.ViewModels;
using AppRestaurant.Views;
using System;
using System.Collections.Generic;
using LoginViewModel = AppRestaurant.ViewModels.Screen.Auth.LoginViewModel;
using RegisterViewModel = AppRestaurant.ViewModels.Screen.Auth.RegisterViewModel;
using GuestViewModel = AppRestaurant.ViewModels.Screens.GuestViewModel;
using CustomerViewModel = AppRestaurant.ViewModels.Screens.CustomerViewModel;
using EmployeeViewModel = AppRestaurant.ViewModels.Screens.EmployeeViewModel;
using LoginScreen = AppRestaurant.Views.Screen.Auth.LoginScreen;
using RegisterScreen = AppRestaurant.Views.Screen.Auth.RegisterScreen;
using GuestScreen = AppRestaurant.Views.Screens.GuestScreen;
using CustomerScreen = AppRestaurant.Views.Screens.CustomerScreen;
using EmployeeScreen = AppRestaurant.Views.Screens.EmployeeScreen;

namespace AppRestaurant.Services.Navigation
{

    public class NavigationService : INavigationService
    {
        private ContentControl _mainContent;
        private IServiceProvider _serviceProvider;
        private Dictionary<Type, Type> _viewModelToViewMapping;
        
        private static NavigationService _instance;
        public static NavigationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NavigationService();
                }
                return _instance;
            }
        }
        
        private NavigationService() 
        {
            InitializeViewModelToViewMapping();
        }
        
        private void InitializeViewModelToViewMapping()
        {
            _viewModelToViewMapping = new Dictionary<Type, Type>
            {
                { typeof(LoginViewModel), typeof(LoginScreen) },
                { typeof(RegisterViewModel), typeof(RegisterScreen) },
                { typeof(MainWindowViewModel), typeof(MainWindow) },
                { typeof(CustomerViewModel), typeof(CustomerScreen) },
                { typeof(EmployeeViewModel), typeof(EmployeeScreen) },
                { typeof(GuestViewModel), typeof(GuestScreen) }
                // Add more mappings as needed
            };
        }

        public void Initialize(ContentControl mainContent)
        {
            _mainContent = mainContent ?? throw new ArgumentNullException(nameof(mainContent));
        }

        public void Initialize(ContentControl mainContent, IServiceProvider serviceProvider)
        {
            _mainContent = mainContent ?? throw new ArgumentNullException(nameof(mainContent));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        
        public void NavigateTo<TViewModel>(UserControl view)
        {
            if (_mainContent == null)
                throw new InvalidOperationException("Navigation service has not been initialized.");
                
            _mainContent.Content = view;
        }
        public void NavigateTo<TViewModel>(object parameter = null) where TViewModel : ViewModelBase
        {
            Type viewModelType = typeof(TViewModel);
            
            if (_viewModelToViewMapping.TryGetValue(viewModelType, out Type viewType))
            {
                try
                {
                    var view = Activator.CreateInstance(viewType) as UserControl;
                    
                    if (view != null)
                    {
                        ViewModelBase viewModel;
                        
                        if (_serviceProvider != null)
                        {
                            viewModel = _serviceProvider.GetService(viewModelType) as ViewModelBase;
                            
                            if (viewModel == null)
                            {
                                viewModel = Activator.CreateInstance(viewModelType) as ViewModelBase;
                            }
                        }
                        else
                        {
                            // Just create directly if no service provider
                            viewModel = Activator.CreateInstance(viewModelType) as ViewModelBase;
                        }
                        
                        if (viewModel != null)
                        {
                            // Initialize ViewModel with parameter if needed
                            if (parameter != null && viewModel is IParametrizedViewModel parameterizedViewModel)
                            {
                                parameterizedViewModel.Initialize(parameter);
                            }
                            
                            view.DataContext = viewModel;
                        }
                        
                        _mainContent.Content = view;
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to create View or ViewModel for {viewModelType.Name}", ex);
                }
            }
            else
            {
                throw new ArgumentException($"No view mapping found for view model {viewModelType.Name}");
            }
        }
        
        public void ToLoginScreen()
        {
            NavigateTo<LoginViewModel>();
        }
        
        public void ToRegisterScreen()
        {
            NavigateTo<RegisterViewModel>();
        }
                
        public void ToGuestScreen()
        {
            NavigateTo<GuestViewModel>();
        }
        
        public void ToCustomerScreen()
        {
            NavigateTo<CustomerViewModel>();
        }
        
        public void ToEmployeeScreen()
        {
            NavigateTo<EmployeeViewModel>();
        }
    }
}