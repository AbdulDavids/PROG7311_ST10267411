using System.ComponentModel.DataAnnotations;

namespace PROG7311_POE_ST10267411.Models
{
    /// <summary>
    /// represents a farm product in the system
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;
        
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Production Date")]
        public DateTime ProductionDate { get; set; }
        
        // Foreign key for Farmer
        public int FarmerId { get; set; }
        
        // Navigation property
        public Farmer? Farmer { get; set; }
    }
} 