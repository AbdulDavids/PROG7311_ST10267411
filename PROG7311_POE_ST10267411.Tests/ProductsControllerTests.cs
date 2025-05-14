using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PROG7311_POE_ST10267411.Controllers;
using PROG7311_POE_ST10267411.Data;
using PROG7311_POE_ST10267411.Models;
using PROG7311_POE_ST10267411.ViewModels;
using System.Security.Claims;

namespace PROG7311_POE_ST10267411.Tests;

/// <summary>
/// unit tests for the products controller
/// </summary>
public class ProductsControllerTests
{
    /// <summary>
    /// tests that a farmer can successfully create a product
    /// </summary>
    [Fact]
    public async Task Create_ValidProduct_ReturnsRedirectToAction()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_Products_Create")
            .Options;

        // Create test data
        using (var context = new ApplicationDbContext(options))
        {
            // Add a test farmer
            var farmer = new Farmer
            {
                Id = 1,
                Name = "Test Farmer",
                Email = "test@example.com",
                Phone = "123-456-7890",
                UserId = "user-id-1"
            };
            context.Farmers.Add(farmer);
            await context.SaveChangesAsync();
        }

        // Setup user manager mock
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStore.Object, null, null, null, null, null, null, null, null);
            
        var testUser = new ApplicationUser { Id = "user-id-1" };
        userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(testUser);

        using (var context = new ApplicationDbContext(options))
        {
            // Create the controller
            var controller = new ProductsController(context, userManagerMock.Object);
            
            // Setup controller context
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "user-id-1"),
                        new Claim(ClaimTypes.Role, "Farmer")
                    }, "test"))
                }
            };

            // Create a valid product view model
            var productViewModel = new ProductViewModel
            {
                Name = "Test Product",
                Category = "Test Category",
                ProductionDate = DateTime.Now,
                FarmerId = 1
            };

            // Act
            var result = await controller.Create(productViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("My", redirectToActionResult.ActionName);

            // Verify product was created
            var product = await context.Products.FirstOrDefaultAsync(p => p.Name == "Test Product");
            Assert.NotNull(product);
            Assert.Equal("Test Category", product.Category);
            Assert.Equal(1, product.FarmerId);
        }
    }

    /// <summary>
    /// tests that the my products page shows only the logged-in farmer's products
    /// </summary>
    [Fact]
    public async Task My_ReturnsViewWithFarmerProducts()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_Products_My")
            .Options;

        // Create test data
        using (var context = new ApplicationDbContext(options))
        {
            // Add test farmers
            var farmer1 = new Farmer
            {
                Id = 1,
                Name = "Test Farmer 1",
                Email = "farmer1@example.com",
                Phone = "123-456-7890",
                UserId = "user-id-1"
            };

            var farmer2 = new Farmer
            {
                Id = 2,
                Name = "Test Farmer 2",
                Email = "farmer2@example.com",
                Phone = "987-654-3210",
                UserId = "user-id-2"
            };

            context.Farmers.AddRange(farmer1, farmer2);

            // Add test products for both farmers
            context.Products.AddRange(
                new Product
                {
                    Id = 1,
                    Name = "Product 1",
                    Category = "Category 1",
                    ProductionDate = DateTime.Now.AddDays(-10),
                    FarmerId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Product 2",
                    Category = "Category 2",
                    ProductionDate = DateTime.Now.AddDays(-5),
                    FarmerId = 1
                },
                new Product
                {
                    Id = 3,
                    Name = "Product 3",
                    Category = "Category 3",
                    ProductionDate = DateTime.Now,
                    FarmerId = 2
                }
            );

            await context.SaveChangesAsync();
        }

        // Setup user manager mock
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStore.Object, null, null, null, null, null, null, null, null);
            
        var testUser = new ApplicationUser { Id = "user-id-1" };
        userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(testUser);

        using (var context = new ApplicationDbContext(options))
        {
            // Create the controller
            var controller = new ProductsController(context, userManagerMock.Object);
            
            // Setup controller context
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "user-id-1"),
                        new Claim(ClaimTypes.Role, "Farmer")
                    }, "test"))
                }
            };

            // Act
            var result = await controller.My();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ProductDetailsViewModel>>(viewResult.Model);
            Assert.Equal(2, model.Count()); // Only farmer 1's products
            Assert.Contains(model, p => p.Name == "Product 1");
            Assert.Contains(model, p => p.Name == "Product 2");
            Assert.DoesNotContain(model, p => p.Name == "Product 3");
        }
    }

    /// <summary>
    /// tests that the index page correctly filters products by date range
    /// </summary>
    [Fact]
    public async Task Index_FiltersProductsByDateRange()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_Products_Index")
            .Options;

        // Create test data
        var today = DateTime.Today;
        using (var context = new ApplicationDbContext(options))
        {
            // Add test farmers
            var farmer = new Farmer
            {
                Id = 1,
                Name = "Test Farmer",
                Email = "farmer@example.com",
                Phone = "123-456-7890"
            };
            context.Farmers.Add(farmer);

            // Add test products with different dates
            context.Products.AddRange(
                new Product
                {
                    Id = 1,
                    Name = "Old Product",
                    Category = "Category 1",
                    ProductionDate = today.AddDays(-30),
                    FarmerId = 1,
                    Farmer = farmer
                },
                new Product
                {
                    Id = 2,
                    Name = "Recent Product",
                    Category = "Category 2",
                    ProductionDate = today.AddDays(-5),
                    FarmerId = 1,
                    Farmer = farmer
                },
                new Product
                {
                    Id = 3,
                    Name = "New Product",
                    Category = "Category 1",
                    ProductionDate = today,
                    FarmerId = 1,
                    Farmer = farmer
                }
            );

            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            // Create the controller with a mock UserManager
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);
            
            // Setup controller context for employee role
            var controller = new ProductsController(context, userManager.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Role, "Employee")
                    }, "test"))
                }
            };

            // Act - filter for last 10 days
            var filterModel = new ProductFilterViewModel
            {
                FromDate = today.AddDays(-10),
                ToDate = today
            };
            var result = await controller.Index(filterModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductFilterViewModel>(viewResult.Model);
            
            Assert.Equal(2, model.Products.Count); // Should include Recent and New products
            Assert.Contains(model.Products, p => p.Name == "Recent Product");
            Assert.Contains(model.Products, p => p.Name == "New Product");
            Assert.DoesNotContain(model.Products, p => p.Name == "Old Product");
        }
    }

    /// <summary>
    /// tests that the index page correctly filters products by category
    /// </summary>
    [Fact]
    public async Task Index_FiltersProductsByCategory()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_Products_IndexCategory")
            .Options;

        // Create test data
        using (var context = new ApplicationDbContext(options))
        {
            // Add test farmer
            var farmer = new Farmer
            {
                Id = 1,
                Name = "Test Farmer",
                Email = "farmer@example.com",
                Phone = "123-456-7890"
            };
            context.Farmers.Add(farmer);

            // Add test products with different categories
            context.Products.AddRange(
                new Product
                {
                    Id = 1,
                    Name = "Product 1",
                    Category = "Grains",
                    ProductionDate = DateTime.Now,
                    FarmerId = 1,
                    Farmer = farmer
                },
                new Product
                {
                    Id = 2,
                    Name = "Product 2",
                    Category = "Vegetables",
                    ProductionDate = DateTime.Now,
                    FarmerId = 1,
                    Farmer = farmer
                },
                new Product
                {
                    Id = 3,
                    Name = "Product 3",
                    Category = "Grains",
                    ProductionDate = DateTime.Now,
                    FarmerId = 1,
                    Farmer = farmer
                }
            );

            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            // Create the controller with a mock UserManager
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);
            
            // Setup controller context for employee role
            var controller = new ProductsController(context, userManager.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Role, "Employee")
                    }, "test"))
                }
            };

            // Act - filter by Grains category
            var filterModel = new ProductFilterViewModel
            {
                Category = "Grains"
            };
            var result = await controller.Index(filterModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductFilterViewModel>(viewResult.Model);
            
            Assert.Equal(2, model.Products.Count); // Should include both Grains products
            Assert.Contains(model.Products, p => p.Name == "Product 1");
            Assert.Contains(model.Products, p => p.Name == "Product 3");
            Assert.DoesNotContain(model.Products, p => p.Name == "Product 2");
        }
    }
} 