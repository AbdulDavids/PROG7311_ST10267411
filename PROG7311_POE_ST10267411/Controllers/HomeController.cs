using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG7311_POE_ST10267411.Data;
using PROG7311_POE_ST10267411.Models;
using PROG7311_POE_ST10267411.ViewModels;

namespace PROG7311_POE_ST10267411.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// display home page with role-specific dashboard
    /// </summary>
    public async Task<IActionResult> Index()
    {
        // Only fetch stats for Employee role
        if (User.IsInRole("Employee"))
        {
            var stats = new DashboardStatsViewModel
            {
                TotalFarmers = await _context.Farmers.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                CategoryCount = await _context.Products.Select(p => p.Category).Distinct().CountAsync(),
                RecentProducts = await _context.Products
                    .Include(p => p.Farmer)
                    .OrderByDescending(p => p.ProductionDate)
                    .Take(5)
                    .Select(p => ProductDetailsViewModel.FromProduct(p))
                    .ToListAsync()
            };
            
            return View(stats);
        }
        
        return View();
    }

    /// <summary>
    /// display privacy policy
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// handle errors
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
