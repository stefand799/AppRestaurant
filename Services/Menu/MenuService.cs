using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppRestaurant.Models;
using AppRestaurant.Repositories.Category;
using AppRestaurant.Repositories.Dish;
using AppRestaurant.Repositories.Meal;
using AppRestaurant.Repositories.Allergen;
using AppRestaurant.Repositories.DishInMeal;

namespace AppRestaurant.Services.Menu
{
    public class MenuService : IMenuService
    {
                private readonly ICategoryRepository _categoryRepository;
        private readonly IDishRepository _dishRepository;
        private readonly IMealRepository _mealRepository;
        private readonly IAllergenRepository _allergenRepository;
        private readonly IDishInMealRepository _dishInMealRepository;
        private readonly decimal _mealDiscountPercentage;

        public MenuService(
            ICategoryRepository categoryRepository,
            IDishRepository dishRepository,
            IMealRepository mealRepository,
            IAllergenRepository allergenRepository,
            IDishInMealRepository dishInMealRepository)
        {
            _categoryRepository = categoryRepository;
            _dishRepository = dishRepository;
            _mealRepository = mealRepository;
            _allergenRepository = allergenRepository;
            _dishInMealRepository = dishInMealRepository;
            _mealDiscountPercentage = AppConfiguration.MealDiscountPercentage;
        }

        // Add this method to search dishes by multiple criteria
        public async Task<IEnumerable<Dish>> SearchDishesAsync(
            string searchTerm = null, 
            int? categoryId = null, 
            bool onlyAvailable = false,
            IEnumerable<int> excludeAllergenIds = null)
        {
            var dishes = await _dishRepository.GetAllWithCategoryAndAllergensAsync();
            
            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                dishes = dishes.Where(d => d.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }
            
            if (categoryId.HasValue)
            {
                dishes = dishes.Where(d => d.CategoryId == categoryId.Value);
            }
            
            if (onlyAvailable)
            {
                dishes = dishes.Where(d => d.Availability);
            }
            
            if (excludeAllergenIds != null && excludeAllergenIds.Any())
            {
                dishes = dishes.Where(d => 
                    !d.Allergens.Any(a => excludeAllergenIds.Contains(a.Id)));
            }
            
            return dishes;
        }

        // Add this method to search meals by multiple criteria
        public async Task<IEnumerable<Meal>> SearchMealsAsync(
            string searchTerm = null, 
            int? categoryId = null,
            bool onlyAvailable = false,
            IEnumerable<int> excludeAllergenIds = null)
        {
            var meals = await _mealRepository.GetAllWithDetailsAsync();
            
            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                meals = meals.Where(m => m.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }
            
            if (categoryId.HasValue)
            {
                meals = meals.Where(m => m.CategoryId == categoryId.Value);
            }
            
            // Filter by availability if needed
            if (onlyAvailable)
            {
                var availableMealIds = new List<int>();
                foreach (var meal in meals)
                {
                    if (await CheckMealAvailabilityAsync(meal.Id))
                    {
                        availableMealIds.Add(meal.Id);
                    }
                }
                meals = meals.Where(m => availableMealIds.Contains(m.Id));
            }
            
            // Filter by allergens if needed
            if (excludeAllergenIds != null && excludeAllergenIds.Any())
            {
                meals = meals.Where(m => 
                    !m.DishInMeals.Any(dm => 
                        dm.Dish.Allergens.Any(a => 
                            excludeAllergenIds.Contains(a.Id))));
            }
            
            return meals;
        }

        #region Category Operations
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<Category> GetCategoryWithItemsAsync(int id)
        {
            return await _categoryRepository.GetByIdWithDishesAndMealsAsync(id);
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveAsync();
            return category;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category != null)
            {
                await _categoryRepository.DeleteAsync(category);
                await _categoryRepository.SaveAsync();
            }
        }
        #endregion

        #region Dish Operations
        public async Task<IEnumerable<Dish>> GetAllDishesAsync()
        {
            return await _dishRepository.GetAllWithCategoryAndAllergensAsync();
        }

        public async Task<Dish> GetDishByIdAsync(int id)
        {
            return await _dishRepository.GetByIdWithCategoryAndAllergensAsync(id);
        }

        public async Task<IEnumerable<Dish>> GetDishesByKeywordAsync(string keyword)
        {
            return await _dishRepository.GetByNameContainsAsync(keyword);
        }

        public async Task<IEnumerable<Dish>> GetDishesByCategoryAsync(int categoryId)
        {
            return await _dishRepository.GetByCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<Dish>> GetAvailableDishesAsync()
        {
            return await _dishRepository.GetByAvailabilityAsync(true);
        }

        public async Task<IEnumerable<Dish>> GetLowStockDishesAsync(decimal threshold)
        {
            return await _dishRepository.GetLowStockAsync(threshold);
        }

        public async Task<IEnumerable<Dish>> GetDishesByAllergenAsync(int allergenId, bool exclude)
        {
            return await _dishRepository.GetByAllergenAsync(allergenId, exclude);
        }

        public async Task<Dish> AddDishAsync(Dish dish)
        {
            await _dishRepository.AddAsync(dish);
            await _dishRepository.SaveAsync();
            return dish;
        }

        public async Task UpdateDishAsync(Dish dish)
        {
            await _dishRepository.UpdateAsync(dish);
            await _dishRepository.SaveAsync();
        }

        public async Task DeleteDishAsync(int id)
        {
            var dish = await _dishRepository.GetByIdAsync(id);
            if (dish != null)
            {
                await _dishRepository.DeleteAsync(dish);
                await _dishRepository.SaveAsync();
            }
        }

        public async Task<bool> UpdateDishStockAsync(int dishId, decimal quantity)
        {
            return await _dishRepository.UpdateStockQuantityAsync(dishId, quantity);
        }
        #endregion

        #region Meal Operations
        public async Task<IEnumerable<Meal>> GetAllMealsAsync()
        {
            return await _mealRepository.GetAllWithDetailsAsync();
        }

        public async Task<Meal> GetMealByIdAsync(int id)
        {
            return await _mealRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Meal>> GetMealsByKeywordAsync(string keyword)
        {
            return await _mealRepository.GetByNameContainsAsync(keyword);
        }

        public async Task<IEnumerable<Meal>> GetMealsByCategoryAsync(int categoryId)
        {
            return await _mealRepository.GetByCategoryAsync(categoryId);
        }

        public async Task<bool> CheckMealAvailabilityAsync(int mealId)
        {
            return await _mealRepository.CheckAvailabilityAsync(mealId);
        }

        public async Task<decimal> CalculateMealPriceAsync(int mealId)
        {
            return await _mealRepository.CalculateMealPriceAsync(mealId, _mealDiscountPercentage);
        }

        public async Task<Meal> AddMealAsync(Meal meal)
        {
            await _mealRepository.AddAsync(meal);
            await _mealRepository.SaveAsync();
            return meal;
        }

        public async Task UpdateMealAsync(Meal meal)
        {
            await _mealRepository.UpdateAsync(meal);
            await _mealRepository.SaveAsync();
        }

        public async Task DeleteMealAsync(int id)
        {
            var meal = await _mealRepository.GetByIdAsync(id);
            if (meal != null)
            {
                await _mealRepository.DeleteAsync(meal);
                await _mealRepository.SaveAsync();
            }
        }
        #endregion

        #region Allergen Operations
        public async Task<IEnumerable<Allergen>> GetAllAllergensAsync()
        {
            return await _allergenRepository.GetAllAsync();
        }

        public async Task<Allergen> GetAllergenByIdAsync(int id)
        {
            return await _allergenRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Allergen>> GetAllergensByDishIdAsync(int dishId)
        {
            return await _allergenRepository.GetByDishIdAsync(dishId);
        }

        public async Task<Allergen> AddAllergenAsync(Allergen allergen)
        {
            await _allergenRepository.AddAsync(allergen);
            await _allergenRepository.SaveAsync();
            return allergen;
        }

        public async Task UpdateAllergenAsync(Allergen allergen)
        {
            await _allergenRepository.UpdateAsync(allergen);
            await _allergenRepository.SaveAsync();
        }

        public async Task DeleteAllergenAsync(int id)
        {
            var allergen = await _allergenRepository.GetByIdAsync(id);
            if (allergen != null)
            {
                await _allergenRepository.DeleteAsync(allergen);
                await _allergenRepository.SaveAsync();
            }
        }

        public async Task AddAllergenToDishAsync(int allergenId, int dishId)
        {
            await _allergenRepository.AddAllergenToDishAsync(allergenId, dishId);
        }

        public async Task RemoveAllergenFromDishAsync(int allergenId, int dishId)
        {
            await _allergenRepository.RemoveAllergenFromDishAsync(allergenId, dishId);
        }
        #endregion

        #region DishInMeal Operations
        public async Task<IEnumerable<DishInMeal>> GetDishesByMealIdAsync(int mealId)
        {
            return await _dishInMealRepository.GetByMealIdAsync(mealId);
        }

        public async Task AddDishToMealAsync(int mealId, int dishId, decimal servingSize, decimal price)
        {
            await _dishInMealRepository.AddDishToMealAsync(mealId, dishId, servingSize, price);
        }

        public async Task RemoveDishFromMealAsync(int mealId, int dishId)
        {
            await _dishInMealRepository.RemoveDishFromMealAsync(mealId, dishId);
        }
        #endregion
    }
}