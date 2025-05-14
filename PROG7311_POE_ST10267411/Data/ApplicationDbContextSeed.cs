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

            // Create employee user - Elon for energy company
            var employeeUser = new ApplicationUser
            {
                UserName = "elon@gmail.com",
                Email = "elon@gmail.com",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(employeeUser.Email) == null)
            {
                await userManager.CreateAsync(employeeUser, "Password123!");
                await userManager.AddToRoleAsync(employeeUser, "Employee");
            }

            // Create demo farmer users - FAANG CEOs as farmers
            var farmer1 = new ApplicationUser
            {
                UserName = "sundar@gmail.com",
                Email = "sundar@gmail.com",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(farmer1.Email) == null)
            {
                await userManager.CreateAsync(farmer1, "Password123!");
                await userManager.AddToRoleAsync(farmer1, "Farmer");
            }

            var farmer2 = new ApplicationUser
            {
                UserName = "jeff@gmail.com",
                Email = "jeff@gmail.com",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(farmer2.Email) == null)
            {
                await userManager.CreateAsync(farmer2, "Password123!");
                await userManager.AddToRoleAsync(farmer2, "Farmer");
            }
            
            var farmer3 = new ApplicationUser
            {
                UserName = "tim@gmail.com",
                Email = "tim@gmail.com",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(farmer3.Email) == null)
            {
                await userManager.CreateAsync(farmer3, "Password123!");
                await userManager.AddToRoleAsync(farmer3, "Farmer");
            }
            
            var farmer4 = new ApplicationUser
            {
                UserName = "mark@gmail.com",
                Email = "mark@gmail.com",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(farmer4.Email) == null)
            {
                await userManager.CreateAsync(farmer4, "Password123!");
                await userManager.AddToRoleAsync(farmer4, "Farmer");
            }
        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            // Only seed if no farmers exist
            if (!context.Farmers.Any())
            {
                // Get the demo farmer users
                var farmer1User = await userManager.FindByEmailAsync("sundar@gmail.com");
                var farmer2User = await userManager.FindByEmailAsync("jeff@gmail.com");
                var farmer3User = await userManager.FindByEmailAsync("tim@gmail.com");
                var farmer4User = await userManager.FindByEmailAsync("mark@gmail.com");

                // Create sample farmers - FAANG CEOs
                var farmer1 = new Farmer
                {
                    Name = "Sundar Pichai",
                    Email = "sundar@gmail.com",
                    Phone = "650-253-0000",
                    UserId = farmer1User?.Id
                };

                var farmer2 = new Farmer
                {
                    Name = "Jeff Bezos",
                    Email = "jeff@gmail.com",
                    Phone = "206-266-1000",
                    UserId = farmer2User?.Id
                };
                
                var farmer3 = new Farmer
                {
                    Name = "Tim Cook",
                    Email = "tim@gmail.com",
                    Phone = "408-996-1010",
                    UserId = farmer3User?.Id
                };
                
                var farmer4 = new Farmer
                {
                    Name = "Mark Zuckerberg",
                    Email = "mark@gmail.com",
                    Phone = "650-853-1300",
                    UserId = farmer4User?.Id
                };

                context.Farmers.Add(farmer1);
                context.Farmers.Add(farmer2);
                context.Farmers.Add(farmer3);
                context.Farmers.Add(farmer4);
                await context.SaveChangesAsync();

                // Create sample products for Sundar Pichai
                var productsForFarmer1 = new List<Product>
                {
                    new Product
                    {
                        Name = "Yellow Corn",
                        Category = "Grains",
                        ProductionDate = DateTime.Now.AddDays(-44),
                        FarmerId = farmer1.Id
                    },
                    new Product
                    {
                        Name = "Organic Soybeans",
                        Category = "Legumes",
                        ProductionDate = DateTime.Now.AddDays(-77),
                        FarmerId = farmer1.Id
                    },
                    new Product
                    {
                        Name = "Winter Wheat",
                        Category = "Grains",
                        ProductionDate = DateTime.Now.AddDays(-25),
                        FarmerId = farmer1.Id
                    }
                };

                // Create sample products for Jeff Bezos
                var productsForFarmer2 = new List<Product>
                {
                    new Product
                    {
                        Name = "Fresh Carrots",
                        Category = "Vegetables",
                        ProductionDate = DateTime.Now.AddDays(-33),
                        FarmerId = farmer2.Id
                    },
                    new Product
                    {
                        Name = "Sugarcane",
                        Category = "Sugar Crops",
                        ProductionDate = DateTime.Now.AddDays(-18),
                        FarmerId = farmer2.Id
                    },
                    new Product
                    {
                        Name = "Biofuel Corn",
                        Category = "Biomass",
                        ProductionDate = DateTime.Now.AddDays(-11),
                        FarmerId = farmer2.Id
                    },
                    new Product
                    {
                        Name = "Sweet Corn",
                        Category = "Grains",
                        ProductionDate = DateTime.Now.AddDays(-5),
                        FarmerId = farmer2.Id
                    }
                };
                
                // Create sample products for Tim Cook
                var productsForFarmer3 = new List<Product>
                {
                    new Product
                    {
                        Name = "Oranges",
                        Category = "Fruits",
                        ProductionDate = DateTime.Now.AddDays(-55),
                        FarmerId = farmer3.Id
                    },
                    new Product
                    {
                        Name = "Roma Tomatoes",
                        Category = "Vegetables",
                        ProductionDate = DateTime.Now.AddDays(-37),
                        FarmerId = farmer3.Id
                    },
                    new Product
                    {
                        Name = "Red Beets",
                        Category = "Vegetables",
                        ProductionDate = DateTime.Now.AddDays(-14),
                        FarmerId = farmer3.Id
                    }
                };
                
                // Create sample products for Mark Zuckerberg
                var productsForFarmer4 = new List<Product>
                {
                    new Product
                    {
                        Name = "Switchgrass",
                        Category = "Biomass",
                        ProductionDate = DateTime.Now.AddDays(-41),
                        FarmerId = farmer4.Id
                    },
                    new Product
                    {
                        Name = "Red Wheat",
                        Category = "Grains",
                        ProductionDate = DateTime.Now.AddDays(-22),
                        FarmerId = farmer4.Id
                    },
                    new Product
                    {
                        Name = "Industrial Hemp",
                        Category = "Biomass",
                        ProductionDate = DateTime.Now.AddDays(-8),
                        FarmerId = farmer4.Id
                    },
                    new Product
                    {
                        Name = "Millet",
                        Category = "Grains",
                        ProductionDate = DateTime.Now.AddDays(-3),
                        FarmerId = farmer4.Id
                    }
                };

                context.Products.AddRange(productsForFarmer1);
                context.Products.AddRange(productsForFarmer2);
                context.Products.AddRange(productsForFarmer3);
                context.Products.AddRange(productsForFarmer4);
                await context.SaveChangesAsync();
            }
        }
    }
} 