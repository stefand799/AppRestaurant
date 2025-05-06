using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using AppRestaurant.ViewModels;
using AppRestaurant.Views;
using AppRestaurant.Services.Navigation;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AppRestaurant;

public partial class App : Application
{
    // Static property to store the service provider
    public static ServiceProvider ServiceProvider { get; set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit
            DisableAvaloniaDataAnnotationValidation();
            
            var mainWindow = new MainWindow();
            desktop.MainWindow = mainWindow;
            
            if (ServiceProvider != null)
            {
                // Initialize navigation after window is ready
                mainWindow.Opened += (s, e) => 
                {
                    var contentControl = mainWindow.FindControl<ContentControl>("MainContent");
                    if (contentControl != null)
                    {
                        var navigationService = ServiceProvider.GetRequiredService<INavigationService>();
                        navigationService.Initialize(contentControl, ServiceProvider);
                        navigationService.ToLoginScreen();
                    }
                    else
                    {
                        Console.WriteLine("Error: MainContent control not found in MainWindow");
                    }
                };
            }
            else
            {
                Console.WriteLine("Warning: ServiceProvider not configured");
                mainWindow.DataContext = new MainWindowViewModel();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}   