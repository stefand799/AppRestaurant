using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppRestaurant.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required, MaxLength(50), EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public byte[] PasswordHash { get; set; }
        
        [Required]
        public byte[] PasswordSalt { get; set; }
        
        [Required, MaxLength(50)]
        public string FirstName { get; set; }
        
        [Required, MaxLength(50)]
        public string LastName { get; set; }
        
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        
        public string Address { get; set; }
        
        [Required, MaxLength(20)]
        public string Role { get; set; }
        
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}