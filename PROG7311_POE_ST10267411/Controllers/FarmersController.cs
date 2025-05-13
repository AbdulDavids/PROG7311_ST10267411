using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG7311_POE_ST10267411.Data;
using PROG7311_POE_ST10267411.Models;
using PROG7311_POE_ST10267411.ViewModels;

namespace PROG7311_POE_ST10267411.Controllers
{
    [Authorize]
    public class FarmersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public FarmersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        /// <summary>
        /// display list of all farmers (employee access)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Index()
        {
            var farmers = await _context.Farmers
                .Include(f => f.Products)
                .ToListAsync();
                
            var viewModel = farmers.Select(f => new FarmerDetailsViewModel
            {
                Id = f.Id,
                Name = f.Name,
                Email = f.Email,
                Phone = f.Phone,
                ProductCount = f.Products.Count
            }).ToList();
            
            return View(viewModel);
        }
        
        /// <summary>
        /// display details of a specific farmer (employee access)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Details(int id)
        {
            var farmer = await _context.Farmers
                .Include(f => f.Products)
                .FirstOrDefaultAsync(f => f.Id == id);
                
            if (farmer == null)
            {
                return NotFound();
            }
            
            var viewModel = new FarmerDetailsViewModel
            {
                Id = farmer.Id,
                Name = farmer.Name,
                Email = farmer.Email,
                Phone = farmer.Phone,
                ProductCount = farmer.Products.Count
            };
            
            return View(viewModel);
        }
        
        /// <summary>
        /// display form to create a new farmer (employee access)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Employee")]
        public IActionResult Create()
        {
            return View(new FarmerViewModel());
        }
        
        /// <summary>
        /// handle form submission to create a new farmer (employee access)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FarmerViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if a farmer with this email already exists
                var existingFarmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.Email == model.Email);
                    
                if (existingFarmer != null)
                {
                    ModelState.AddModelError("Email", "a farmer with this email already exists");
                    return View(model);
                }
                
                // If creating an account for the farmer
                string? userId = null;
                if (model.CreateAccount && !string.IsNullOrEmpty(model.Password))
                {
                    // Check if a user with this email already exists
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Email", "a user with this email already exists");
                        return View(model);
                    }
                    
                    // Create the user
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        EmailConfirmed = true
                    };
                    
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        // Assign Farmer role
                        await _userManager.AddToRoleAsync(user, "Farmer");
                        userId = user.Id;
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }
                
                // Create the farmer
                var farmer = new Farmer
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    UserId = userId
                };
                
                _context.Farmers.Add(farmer);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            
            return View(model);
        }
        
        /// <summary>
        /// display form to edit a farmer (employee access)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int id)
        {
            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer == null)
            {
                return NotFound();
            }
            
            var viewModel = new FarmerViewModel
            {
                Id = farmer.Id,
                Name = farmer.Name,
                Email = farmer.Email,
                Phone = farmer.Phone
            };
            
            return View(viewModel);
        }
        
        /// <summary>
        /// handle form submission to edit a farmer (employee access)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FarmerViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var farmer = await _context.Farmers.FindAsync(id);
                    if (farmer == null)
                    {
                        return NotFound();
                    }
                    
                    // Check if changing email to one that already exists
                    if (farmer.Email != model.Email)
                    {
                        var existingFarmer = await _context.Farmers
                            .FirstOrDefaultAsync(f => f.Email == model.Email && f.Id != id);
                            
                        if (existingFarmer != null)
                        {
                            ModelState.AddModelError("Email", "a farmer with this email already exists");
                            return View(model);
                        }
                    }
                    
                    farmer.Name = model.Name;
                    farmer.Email = model.Email;
                    farmer.Phone = model.Phone;
                    
                    _context.Update(farmer);
                    await _context.SaveChangesAsync();
                    
                    // If the farmer has a user account, update that email as well
                    if (!string.IsNullOrEmpty(farmer.UserId))
                    {
                        var user = await _userManager.FindByIdAsync(farmer.UserId);
                        if (user != null && user.Email != model.Email)
                        {
                            user.Email = model.Email;
                            user.UserName = model.Email;
                            await _userManager.UpdateAsync(user);
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await FarmerExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }
            
            return View(model);
        }
        
        /// <summary>
        /// display form for a farmer to create their own profile
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> CreateProfile()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }
            
            // Check if farmer profile already exists
            var existingFarmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.UserId == currentUser.Id);
                
            if (existingFarmer != null)
            {
                // If profile already exists, redirect to products
                return RedirectToAction("My", "Products");
            }
            
            // Pre-fill email from user account
            var viewModel = new FarmerViewModel
            {
                Email = currentUser.Email
            };
            
            return View(viewModel);
        }
        
        /// <summary>
        /// handle form submission for a farmer creating their own profile
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Farmer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfile(FarmerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Challenge();
                }
                
                // Check if a farmer with this email already exists
                var existingFarmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.Email == model.Email);
                    
                if (existingFarmer != null)
                {
                    ModelState.AddModelError("Email", "a farmer with this email already exists");
                    return View(model);
                }
                
                // Create the farmer profile
                var farmer = new Farmer
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    UserId = currentUser.Id
                };
                
                _context.Farmers.Add(farmer);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("My", "Products");
            }
            
            return View(model);
        }
        
        private async Task<bool> FarmerExists(int id)
        {
            return await _context.Farmers.AnyAsync(e => e.Id == id);
        }
    }
} 