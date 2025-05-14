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
/// unit tests for the farmers controller
/// </summary>
public class FarmersControllerTests
{
    /// <summary>
    /// tests that the index page returns all farmers
    /// </summary>
    [Fact]
    public async Task Index_ReturnsAllFarmers()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_Farmers_Index")
            .Options;

        // Create test data
        using (var context = new ApplicationDbContext(options))
        {
            // Add test farmers with products
            var farmer1 = new Farmer
            {
                Id = 1,
                Name = "Test Farmer 1",
                Email = "farmer1@example.com",
                Phone = "123-456-7890"
            };

            var farmer2 = new Farmer
            {
                Id = 2,
                Name = "Test Farmer 2",
                Email = "farmer2@example.com",
                Phone = "987-654-3210"
            };

            context.Farmers.AddRange(farmer1, farmer2);

            // Add test products for both farmers
            context.Products.AddRange(
                new Product
                {
                    Id = 1,
                    Name = "Product 1",
                    Category = "Category 1",
                    ProductionDate = DateTime.Now,
                    FarmerId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Product 2",
                    Category = "Category 2",
                    ProductionDate = DateTime.Now,
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

        using (var context = new ApplicationDbContext(options))
        {
            // Create the controller with a mock UserManager
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);
            
            var controller = new FarmersController(context, userManager.Object);
            
            // Setup controller context for employee role
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

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<FarmerDetailsViewModel>>(viewResult.Model);
            
            Assert.Equal(2, model.Count());
            Assert.Contains(model, f => f.Name == "Test Farmer 1" && f.ProductCount == 2);
            Assert.Contains(model, f => f.Name == "Test Farmer 2" && f.ProductCount == 1);
        }
    }

    /// <summary>
    /// tests that an employee can successfully create a farmer
    /// </summary>
    [Fact]
    public async Task Create_ValidFarmer_ReturnsRedirectToAction()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_Farmers_Create")
            .Options;

        // Setup user manager mock
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStore.Object, null, null, null, null, null, null, null, null);

        using (var context = new ApplicationDbContext(options))
        {
            // Create the controller
            var controller = new FarmersController(context, userManagerMock.Object);
            
            // Setup controller context for employee role
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

            // Create a valid farmer view model
            var farmerViewModel = new FarmerViewModel
            {
                Name = "New Farmer",
                Email = "new@example.com",
                Phone = "555-123-4567"
            };

            // Act
            var result = await controller.Create(farmerViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // Verify farmer was created
            var farmer = await context.Farmers.FirstOrDefaultAsync(f => f.Email == "new@example.com");
            Assert.NotNull(farmer);
            Assert.Equal("New Farmer", farmer.Name);
            Assert.Equal("555-123-4567", farmer.Phone);
        }
    }

    /// <summary>
    /// tests that the details page returns the correct farmer information
    /// </summary>
    [Fact]
    public async Task Details_ReturnsFarmerDetails()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_Farmers_Details")
            .Options;

        // Create test data
        using (var context = new ApplicationDbContext(options))
        {
            // Add test farmer with products
            var farmer = new Farmer
            {
                Id = 1,
                Name = "Test Farmer",
                Email = "test@example.com",
                Phone = "123-456-7890"
            };

            context.Farmers.Add(farmer);

            // Add test products
            context.Products.AddRange(
                new Product
                {
                    Id = 1,
                    Name = "Product 1",
                    Category = "Category 1",
                    ProductionDate = DateTime.Now,
                    FarmerId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Product 2",
                    Category = "Category 2",
                    ProductionDate = DateTime.Now,
                    FarmerId = 1
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
            
            var controller = new FarmersController(context, userManager.Object);
            
            // Setup controller context for employee role
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

            // Act
            var result = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<FarmerDetailsViewModel>(viewResult.Model);
            
            Assert.Equal(1, model.Id);
            Assert.Equal("Test Farmer", model.Name);
            Assert.Equal("test@example.com", model.Email);
            Assert.Equal("123-456-7890", model.Phone);
            Assert.Equal(2, model.ProductCount); // Should have 2 products
        }
    }

    /// <summary>
    /// tests that the details page returns not found for an invalid farmer id
    /// </summary>
    [Fact]
    public async Task Details_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_Farmers_DetailsNotFound")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            // Create the controller with a mock UserManager
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);
            
            var controller = new FarmersController(context, userManager.Object);
            
            // Setup controller context for employee role
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

            // Act - request a farmer that doesn't exist
            var result = await controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }

    /// <summary>
    /// tests that create validation works
    /// </summary>
    [Fact]
    public async Task Create_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_Farmers_CreateInvalid")
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            // Create the controller with a mock UserManager
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);
            
            var controller = new FarmersController(context, userManager.Object);
            
            // Setup controller context for employee role
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

            // Create an invalid farmer view model (missing required fields)
            var farmerViewModel = new FarmerViewModel();
            
            // Add model error
            controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await controller.Create(farmerViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<FarmerViewModel>(viewResult.Model);
            Assert.False(controller.ModelState.IsValid);
        }
    }
} 