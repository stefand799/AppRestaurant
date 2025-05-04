using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AppRestaurant.Models;

namespace AppRestaurant.Data
{
    public static class DbInitializer
    {
        public static async Task SeedDatabaseAsync(AppRestaurantDbContext context, PasswordHasher<User> passwordHasher)
        {
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Appetizers" },
                    new Category { Name = "Main Dishes" },
                    new Category { Name = "Sides" },
                    new Category { Name = "Desserts" },
                    new Category { Name = "Beverages" }
                };
                
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
                Console.WriteLine("Added categories");
            }

            if (!await context.Categories.AnyAsync())
            {
                var allergens = new List<Allergen>
                {
                    new Allergen { Name = "Gluten" },
                    new Allergen { Name = "Dairy" },
                    new Allergen { Name = "Nuts" },
                    new Allergen { Name = "Shellfish" },
                    new Allergen { Name = "Eggs" },
                    new Allergen { Name = "Soy" }
                };

                await context.Allergens.AddRangeAsync(allergens);
                await context.SaveChangesAsync();
                Console.WriteLine("Added allergens");
            }
            
            if (!await context.Users.AnyAsync())
            {
                var adminUser = new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@example.com",
                    PhoneNumber = "555-1234",
                    Address = "123 Admin St",
                    Role = "Employee"
                };
                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "admin123");

                var customerUser = new User
                {
                    FirstName = "Customer",
                    LastName = "User",
                    Email = "customer@example.com",
                    PhoneNumber = "555-5678",
                    Address = "456 Customer Ave",
                    Role = "Customer"
                };
                customerUser.PasswordHash = passwordHasher.HashPassword(customerUser, "customer123");

                await context.Users.AddRangeAsync(new[] { adminUser, customerUser });
                await context.SaveChangesAsync();
                Console.WriteLine("Added default users");
            }
            
            if (!await context.Dishes.AnyAsync())
            {
                var categories = await context.Categories.ToListAsync();
                var allergens = await context.Allergens.ToListAsync();

                var dishes = new List<Dish>
                {
                    new Dish
                    {
                        Name = "Caesar Salad",
                        Price = 9.99m,
                        ServingSize = 250,
                        ServingUnit = "g",
                        StockQuantity = 50,
                        Availability = true,
                        CategoryId = categories.First(c => c.Name == "Appetizers").Id,
                        Allergens = new List<Allergen>
                        {
                            allergens.First(a => a.Name == "Gluten"),
                            allergens.First(a => a.Name == "Dairy")
                        }
                    },
                    new Dish
                    {
                        Name = "Spaghetti Carbonara",
                        Price = 14.99m,
                        ServingSize = 350,
                        ServingUnit = "g",
                        StockQuantity = 40,
                        Availability = true,
                        CategoryId = categories.First(c => c.Name == "Main Dishes").Id,
                        Allergens = new List<Allergen>
                        {
                            allergens.First(a => a.Name == "Gluten"),
                            allergens.First(a => a.Name == "Dairy"),
                            allergens.First(a => a.Name == "Eggs")
                        }
                    },
                    new Dish
                    {
                        Name = "French Fries",
                        Price = 4.99m,
                        ServingSize = 200,
                        ServingUnit = "g",
                        StockQuantity = 100,
                        Availability = true,
                        CategoryId = categories.First(c => c.Name == "Sides").Id,
                        Allergens = new List<Allergen>
                        {
                            allergens.First(a => a.Name == "Gluten")
                        }
                    },
                    new Dish
                    {
                        Name = "Chocolate Cake",
                        Price = 7.99m,
                        ServingSize = 150,
                        ServingUnit = "g",
                        StockQuantity = 30,
                        Availability = true,
                        CategoryId = categories.First(c => c.Name == "Desserts").Id,
                        Allergens = new List<Allergen>
                        {
                            allergens.First(a => a.Name == "Gluten"),
                            allergens.First(a => a.Name == "Dairy"),
                            allergens.First(a => a.Name == "Eggs"),
                            allergens.First(a => a.Name == "Nuts")
                        }
                    },
                    new Dish
                    {
                        Name = "Coca Cola",
                        Price = 2.99m,
                        ServingSize = 330,
                        ServingUnit = "ml",
                        StockQuantity = 200,
                        Availability = true,
                        CategoryId = categories.First(c => c.Name == "Beverages").Id,
                        Allergens = new List<Allergen>()
                    }
                };

                await context.Dishes.AddRangeAsync(dishes);
                await context.SaveChangesAsync();
                Console.WriteLine("Added sample dishes");
            }
        }
    }
}