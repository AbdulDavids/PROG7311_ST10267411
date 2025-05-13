using System.ComponentModel.DataAnnotations;

namespace PROG7311_POE_ST10267411.ViewModels
{
    /// <summary>
    /// model for creating or editing a farmer
    /// </summary>
    public class FarmerViewModel
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
        
        // If creating a new user account for the farmer
        public bool CreateAccount { get; set; } = false;
        
        [StringLength(100, ErrorMessage = "the {0} must be at least {2} and at max {1} characters long", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "the password and confirmation password do not match")]
        [Display(Name = "Confirm password")]
        public string? ConfirmPassword { get; set; }
    }
    
    /// <summary>
    /// model for displaying a farmer with product counts
    /// </summary>
    public class FarmerDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
} 