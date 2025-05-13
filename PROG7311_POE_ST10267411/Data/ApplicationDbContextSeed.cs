using Microsoft.AspNetCore.Identity;
using PROG7311_POE_ST10267411.Models;

namespace PROG7311_POE_ST10267411.Data
{
    /// <summary>
    /// seeds the database with initial data
    /// </summary>
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create roles if they don't exist
            string[] roles = new[] { "Farmer", "Employee" };
            
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create employee user
            var employeeUser = new ApplicationUser
            {
                UserName = "employee@demo.local",
                Email = "employee@demo.local",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(employeeUser.Email) == null)
            {
                await userManager.CreateAsync(employeeUser, "Password123!");
                await userManager.AddToRoleAsync(employeeUser, "Employee");
            }

            // Create demo farmer users
            var farmer1 = new ApplicationUser
            {
                UserName = "farmer1@demo.local",
                Email = "farmer1@demo.local",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(farmer1.Email) == null)
            {
                await userManager.CreateAsync(farmer1, "Password123!");
                await userManager.AddToRoleAsync(farmer1, "Farmer");
            }

            var farmer2 = new ApplicationUser
            {
                UserName = "farmer2@demo.local",
                Email = "farmer2@demo.local",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(farmer2.Email) == null)
            {
                await userManager.CreateAsync(farmer2, "Password123!");
                await userManager.AddToRoleAsync(farmer2, "Farmer");
            }
        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            // Only seed if no farmers exist
            if (!context.Farmers.Any())
            {
                // Get the demo farmer users
                var farmer1User = await userManager.FindByEmailAsync("farmer1@demo.local");
                var farmer2User = await userManager.FindByEmailAsync("farmer2@demo.local");

                // Create sample farmers
                var farmer1 = new Farmer
                {
                    Name = "John Smith",
                    Email = "farmer1@demo.local",
                    Phone = "081-555-0123",
                    UserId = farmer1User?.Id
                };

                var farmer2 = new Farmer
                {
                    Name = "Mary Johnson",
                    Email = "farmer2@demo.local",
                    Phone = "082-555-0456",
                    UserId = farmer2User?.Id
                };

                context.Farmers.Add(farmer1);
                context.Farmers.Add(farmer2);
                await context.SaveChangesAsync();

                // Create sample products for farmer 1
                var productsForFarmer1 = new List<Product>
                {
                    new Product
                    {
                        Name = "Organic Corn",
                        Category = "Grains",
                        ProductionDate = DateTime.Now.AddDays(-30),
                        FarmerId = farmer1.Id
                    },
                    new Product
                    {
                        Name = "Sweet Potatoes",
                        Category = "Vegetables",
                        ProductionDate = DateTime.Now.AddDays(-15),
                        FarmerId = farmer1.Id
                    },
                    new Product
                    {
                        Name = "Alfalfa",
                        Category = "Biomass",
                        ProductionDate = DateTime.Now.AddDays(-7),
                        FarmerId = farmer1.Id
                    }
                };

                // Create sample products for farmer 2
                var productsForFarmer2 = new List<Product>
                {
                    new Product
                    {
                        Name = "Sugarcane",
                        Category = "Sugar Crops",
                        ProductionDate = DateTime.Now.AddDays(-45),
                        FarmerId = farmer2.Id
                    },
                    new Product
                    {
                        Name = "Soybeans",
                        Category = "Legumes",
                        ProductionDate = DateTime.Now.AddDays(-20),
                        FarmerId = farmer2.Id
                    },
                    new Product
                    {
                        Name = "Switchgrass",
                        Category = "Biomass",
                        ProductionDate = DateTime.Now.AddDays(-10),
                        FarmerId = farmer2.Id
                    }
                };

                context.Products.AddRange(productsForFarmer1);
                context.Products.AddRange(productsForFarmer2);
                await context.SaveChangesAsync();
            }
        }
    }
} 