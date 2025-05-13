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
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public ProductsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        /// <summary>
        /// display all products for employees with filtering
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Index(ProductFilterViewModel model)
        {
            // Get all products with farmer info
            var query = _context.Products.Include(p => p.Farmer).AsQueryable();
            
            // Apply filters if provided
            if (model.FromDate.HasValue)
            {
                query = query.Where(p => p.ProductionDate >= model.FromDate.Value);
            }
            
            if (model.ToDate.HasValue)
            {
                query = query.Where(p => p.ProductionDate <= model.ToDate.Value);
            }
            
            if (!string.IsNullOrEmpty(model.Category))
            {
                query = query.Where(p => p.Category == model.Category);
            }
            
            if (model.FarmerId.HasValue)
            {
                query = query.Where(p => p.FarmerId == model.FarmerId.Value);
            }
            
            // Get the products and convert to view models
            var products = await query.ToListAsync();
            model.Products = products.Select(ProductDetailsViewModel.FromProduct).ToList();
            
            // Get all distinct categories for the dropdown
            model.Categories = await _context.Products.Select(p => p.Category).Distinct().ToListAsync();
            
            // Get all farmers for the dropdown
            model.Farmers = await _context.Farmers.ToListAsync();
            
            return View(model);
        }
        
        /// <summary>
        /// display products for a specific farmer (employee access)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> FarmerProducts(int id)
        {
            var farmer = await _context.Farmers
                .Include(f => f.Products)
                .FirstOrDefaultAsync(f => f.Id == id);
                
            if (farmer == null)
            {
                return NotFound();
            }
            
            var viewModel = new ProductFilterViewModel
            {
                Products = farmer.Products.Select(ProductDetailsViewModel.FromProduct).ToList(),
                FarmerId = farmer.Id
            };
            
            ViewData["FarmerName"] = farmer.Name;
            
            return View(viewModel);
        }
        
        /// <summary>
        /// display current farmer's products
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> My()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }
            
            var farmer = await _context.Farmers
                .Include(f => f.Products)
                .FirstOrDefaultAsync(f => f.UserId == currentUser.Id);
                
            if (farmer == null)
            {
                // The farmer profile doesn't exist yet
                return RedirectToAction("CreateProfile", "Farmers");
            }
            
            var products = farmer.Products.Select(ProductDetailsViewModel.FromProduct).ToList();
            
            return View(products);
        }
        
        /// <summary>
        /// display form to create a new product
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }
            
            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.UserId == currentUser.Id);
                
            if (farmer == null)
            {
                // The farmer profile doesn't exist yet
                return RedirectToAction("CreateProfile", "Farmers");
            }
            
            var model = new ProductViewModel
            {
                FarmerId = farmer.Id,
                ProductionDate = DateTime.Today
            };
            
            return View(model);
        }
        
        /// <summary>
        /// handle form submission to create a new product
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Farmer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Challenge();
                }
                
                var farmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.UserId == currentUser.Id);
                    
                if (farmer == null || farmer.Id != model.FarmerId)
                {
                    return Forbid();
                }
                
                var product = new Product
                {
                    Name = model.Name,
                    Category = model.Category,
                    ProductionDate = model.ProductionDate,
                    FarmerId = farmer.Id
                };
                
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(My));
            }
            
            return View(model);
        }
        
        /// <summary>
        /// display form to edit a product
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }
            
            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.UserId == currentUser.Id);
                
            if (farmer == null)
            {
                return NotFound();
            }
            
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);
                
            if (product == null)
            {
                return NotFound();
            }
            
            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                ProductionDate = product.ProductionDate,
                FarmerId = product.FarmerId
            };
            
            return View(model);
        }
        
        /// <summary>
        /// handle form submission to edit a product
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Farmer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Challenge();
                }
                
                var farmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.UserId == currentUser.Id);
                    
                if (farmer == null || farmer.Id != model.FarmerId)
                {
                    return Forbid();
                }
                
                try
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);
                        
                    if (product == null)
                    {
                        return NotFound();
                    }
                    
                    product.Name = model.Name;
                    product.Category = model.Category;
                    product.ProductionDate = model.ProductionDate;
                    
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
                return RedirectToAction(nameof(My));
            }
            
            return View(model);
        }
        
        /// <summary>
        /// display confirmation before deleting a product
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }
            
            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.UserId == currentUser.Id);
                
            if (farmer == null)
            {
                return NotFound();
            }
            
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);
                
            if (product == null)
            {
                return NotFound();
            }
            
            var model = ProductDetailsViewModel.FromProduct(product);
            
            return View(model);
        }
        
        /// <summary>
        /// handle confirmed product deletion
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Farmer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }
            
            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(f => f.UserId == currentUser.Id);
                
            if (farmer == null)
            {
                return NotFound();
            }
            
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);
                
            if (product == null)
            {
                return NotFound();
            }
            
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(My));
        }
        
        private async Task<bool> ProductExists(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }
    }
} 