using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppRestaurant.Models
{
    [Table ("Allergens")]
    public class Allergen
    {
        [Key]
        public int Id { get; set; }
        
        [Required, MaxLength(50)]
        public string Name { get; set; }
        
        public ICollection<Dish> Dishes { get; set; } =  new List<Dish>();
    }
}