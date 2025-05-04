using Microsoft.EntityFrameworkCore;
using AppRestaurant.Models;

namespace AppRestaurant.Data
{
    public class AppRestaurantDbContext : DbContext
    {
        public AppRestaurantDbContext(DbContextOptions<AppRestaurantDbContext> options) : base(options){}
        
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Allergen> Allergens { get; set; }
        public DbSet<DishInMeal> DishInMeals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dish>()
                .HasMany(d => d.Allergens)
                .WithMany(a => a.Dishes)
                .UsingEntity(j => j.ToTable("AllergensDishes"));
            
            modelBuilder.Entity<DishInMeal>()
                .HasOne(dm => dm.Dish)
                .WithMany(d => d.DishInMeals)
                .HasForeignKey(dm => dm.DishId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DishInMeal>()
                .HasOne(dm => dm.Meal)
                .WithMany(m => m.DishInMeals)
                .HasForeignKey(dm => dm.MealId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(d => d.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Meal)
                .WithMany(m => m.OrderItems)
                .HasForeignKey(oi => oi.MealId)
                .IsRequired(false);
        } 
    }
}