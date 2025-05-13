using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PROG7311_POE_ST10267411.Models;

namespace PROG7311_POE_ST10267411.Data
{
    /// <summary>
    /// database context for the application
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all model configurations
            ModelConfiguration.ConfigureModels(modelBuilder);
        }
        
        /// <summary>
        /// configures the database context options
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
            // Ensure the directory for the database exists
            if (!Directory.Exists("App_Data"))
            {
                Directory.CreateDirectory("App_Data");
            }
        }
    }
} 