using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppRestaurant.Models
{
    [Table("DishInMeals")]
    public class DishInMeal
    {
        [Key]
        public int Id { get; set; }
        public int DishId { get; set; }
        public int MealId { get; set; }
        
        [Required]
        public decimal DishServingSize { get; set; }
        
        [Required]
        public decimal DishPrice { get; set; }
        
        [ForeignKey("DishId")]
        public virtual Dish Dish { get; set; }
        [ForeignKey("MealId")]
        public virtual Meal Meal { get; set; }
    }
}