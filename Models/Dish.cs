using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppRestaurant.Models
{
    [Table("Dishes")]
    public class Dish
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        
        [Required, MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public decimal ServingSize { get; set; }
        
        [Required, MaxLength(10)]
        public string ServingUnit { get; set; }
        
        [Required]
        public decimal StockQuantity { get; set; }
        
        [Required]
        public bool Available { get; set; }
        
        [Required, MaxLength(500)]
        public string ImageUrl { get; set; }
        
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public virtual ICollection<Allergen> Allergens { get; set; } = new List<Allergen>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<DishInMeal> DishInMeals { get; set; } = new List<DishInMeal>();
    }
}