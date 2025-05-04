using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppRestaurant.Models
{
    [Table("Meals")]
    public class Meal
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        
        [Required, MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        public decimal BasePrice { get; set; }
        public decimal DiscountPrecentage { get; set; }
        
        [Required, MaxLength(500)]
        public string ImageUrl { get; set; }
        
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public virtual ICollection<DishInMeal> DishInMeals { get; set; } = new List<DishInMeal>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}