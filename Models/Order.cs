using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AppRestaurant.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required, MaxLength(20)]
        public string OrderCode { get; set; }
        
        [Required]
        public decimal TotalPrice { get; set; }
        public decimal TransportCost { get; set; }
        public decimal DiscountPrecentage { get; set; }
        
        [Required, MaxLength(20)]
        public string Status { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDeliveryTime { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}