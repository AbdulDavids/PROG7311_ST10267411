using Microsoft.EntityFrameworkCore;
using PROG7311_POE_ST10267411.Models;

namespace PROG7311_POE_ST10267411.Data
{
    /// <summary>
    /// handles entity framework model configuration
    /// </summary>
    public static class ModelConfiguration
    {
        /// <summary>
        /// configures entity relationships and properties
        /// </summary>
        public static void ConfigureModels(ModelBuilder modelBuilder)
        {
            // Configure Farmer
            modelBuilder.Entity<Farmer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                
                // Configure the relationship with ApplicationUser
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            
            // Configure Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ProductionDate).IsRequired();
                
                // Configure the relationship with Farmer
                entity.HasOne(e => e.Farmer)
                    .WithMany(f => f.Products)
                    .HasForeignKey(e => e.FarmerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
} 