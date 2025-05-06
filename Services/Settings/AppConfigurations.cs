using System;

namespace AppRestaurant
{
    public static class AppConfiguration
    {
        public static string ConnectionString { get; } = 
            "Server=127.0.0.1,14330;Database=AppRestaurant;User Id=sa;Password=Qwerty12345;TrustServerCertificate=True;Encrypt=False;Connection Timeout=30;";
        
        // Other configuration settings
        public static decimal MealDiscountPercentage { get; } = 10m; 
        public static decimal FreeDeliveryThreshold { get; } = 50m;
        public static decimal DeliveryFee { get; } = 5m;
        public static int LowStockThreshold { get; } = 5;
        public static int LoyaltyOrderCount { get; } = 5;
        public static int LoyaltyTimeSpan { get; } = 30; // days
        public static decimal LoyaltyDiscountPercentage { get; } = 15m;
    }
}