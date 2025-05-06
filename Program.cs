using Avalonia;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AppRestaurant.Data;
using AppRestaurant.Repositories.Base;
using AppRestaurant.Repositories.User;
using AppRestaurant.Repositories.Category;
using AppRestaurant.Repositories.Dish;
using AppRestaurant.Repositories.Meal;
using AppRestaurant.Repositories.Allergen;
using AppRestaurant.Repositories.Order;
using AppRestaurant.Repositories.OrderItem;
using AppRestaurant.Repositories.DishInMeal;
using AppRestaurant.Services.Auth;
using AppRestaurant.Services.CurrentUser;
using AppRestaurant.Services.Navigation;
using AppRestaurant.Services.Menu;
using AppRestaurant.Services.Order;
using Microsoft.AspNetCore.Identity;
using AppRestaurant.Models;

namespace AppRestaurant;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Configure services
        var serviceProvider = ConfigureServices();
        
        // Test database connection
        TestDatabaseConnection(serviceProvider);
        
        // Initialize database
        InitializeDatabase(serviceProvider);
        
        // Set the service provider for the App class to use
        App.ServiceProvider = serviceProvider;
        
        // Start app
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
            
    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
    
        // Configure AppRestaurantDbContext
        services.AddDbContext<AppRestaurantDbContext>(options =>
            options.UseSqlServer(AppConfiguration.ConnectionString));
        
        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IDishRepository, DishRepository>();
        services.AddScoped<IMealRepository, MealRepository>();
        services.AddScoped<IAllergenRepository, AllergenRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IDishInMealRepository, DishInMealRepository>();
    
        // Register services
        services.AddSingleton<CurrentUserService>();
        services.AddScoped<ICurrentUserService>(provider => provider.GetRequiredService<CurrentUserService>());
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IOrderService, OrderService>();
    
        // Use the singleton instance of NavigationService
        services.AddSingleton<INavigationService>(provider => NavigationService.Instance);
    
        services.AddSingleton<PasswordHasher<User>>();
    
        // Add ViewModels
        services.AddTransient<ViewModels.Screen.Auth.LoginViewModel>();
        services.AddTransient<ViewModels.Screen.Auth.RegisterViewModel>();
        services.AddTransient<ViewModels.Screens.GuestViewModel>();
        services.AddTransient<ViewModels.Screens.CustomerViewModel>();
        services.AddTransient<ViewModels.Screens.EmployeeViewModel>();
        services.AddTransient<ViewModels.Pages.MenuPageViewModel>();
    
        return services.BuildServiceProvider();
    }   
    
    private static void TestDatabaseConnection(ServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppRestaurantDbContext>();
            
            Console.WriteLine("Testing database connection...");
            
            bool canConnect = dbContext.Database.CanConnect();
            
            if (canConnect)
                Console.WriteLine("Successfully connected to the database!");
            else
                Console.WriteLine("Could not connect to the database.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database connection test failed: {ex.Message}");
        }
    }
    
    private static void InitializeDatabase(ServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppRestaurantDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher<User>>();
            
            Console.WriteLine("Ensuring database is created...");
            dbContext.Database.EnsureCreated();
            
            Console.WriteLine("Seeding database...");
            DbInitializer.SeedDatabaseAsync(dbContext, passwordHasher).GetAwaiter().GetResult();
            
            Console.WriteLine("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}