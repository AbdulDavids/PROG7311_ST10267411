using System.ComponentModel.DataAnnotations;

namespace PROG7311_POE_ST10267411.Models
{
    /// <summary>
    /// represents a farmer in the system
    /// </summary>
    public class Farmer
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
        
        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();
        
        // User ID from Identity
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
} 