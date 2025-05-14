using PROG7311_POE_ST10267411.Models;

namespace PROG7311_POE_ST10267411.ViewModels
{
    /// <summary>
    /// model for dashboard statistics display
    /// </summary>
    public class DashboardStatsViewModel
    {
        public int TotalFarmers { get; set; }
        public int TotalProducts { get; set; }
        public int CategoryCount { get; set; }
        public List<ProductDetailsViewModel> RecentProducts { get; set; } = new List<ProductDetailsViewModel>();
    }
} 