using System.ComponentModel.DataAnnotations;
using PROG7311_POE_ST10267411.Models;

namespace PROG7311_POE_ST10267411.ViewModels
{
    /// <summary>
    /// model for creating or editing a product
    /// </summary>
    public class ProductViewModel
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
        public DateTime ProductionDate { get; set; } = DateTime.Today;
        
        // This is populated by the controller when needed for a farmer's products
        // and is not directly editable by the user.
        public int FarmerId { get; set; }
    }
    
    /// <summary>
    /// model for displaying a product with farmer details
    /// </summary>
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime ProductionDate { get; set; }
        public int FarmerId { get; set; }
        public string FarmerName { get; set; } = string.Empty;
        
        // Static constructor to create from a Product entity
        public static ProductDetailsViewModel FromProduct(Product product)
        {
            return new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                ProductionDate = product.ProductionDate,
                FarmerId = product.FarmerId,
                FarmerName = product.Farmer?.Name ?? "Unknown"
            };
        }
    }
    
    /// <summary>
    /// model for filtering products
    /// </summary>
    public class ProductFilterViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }
        
        [Display(Name = "Category")]
        public string? Category { get; set; }
        
        [Display(Name = "Farmer")]
        public int? FarmerId { get; set; }
        
        public List<ProductDetailsViewModel> Products { get; set; } = new List<ProductDetailsViewModel>();
        
        // For the category dropdown
        public List<string> Categories { get; set; } = new List<string>();
        
        // For the farmer dropdown
        public List<Farmer> Farmers { get; set; } = new List<Farmer>();
    }
} 