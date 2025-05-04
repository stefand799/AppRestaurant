using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppRestaurant.Models
{
    [Table("OrderItems")]
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? DishId { get; set; }
        public int? MealId { get; set; }
        
        [Required]
        public int Qunatity { get; set; }
        
        [Required, MaxLength(10)]
        public string Type { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public decimal TotalPrice { get; set; }
        
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        
        [ForeignKey("DishId")]
        public virtual Dish Dish { get; set; }
        
        [ForeignKey("MealId")]
        public virtual Meal Meal { get; set; }
    }
}