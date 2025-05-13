using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROG7311_POE_ST10267411.Models;

namespace PROG7311_POE_ST10267411.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// display home page with role-specific dashboard
    /// </summary>
    public IActionResult Index()
    {
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
