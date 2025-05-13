using Microsoft.EntityFrameworkCore;

namespace PROG7311_POE_ST10267411.Data
{
    /// <summary>
    /// manages database migrations programmatically
    /// </summary>
    public static class MigrationsManager
    {
        /// <summary>
        /// applies pending migrations or creates the database if it doesn't exist
        /// </summary>
        public static void ApplyMigrations(ApplicationDbContext context)
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
            else if (!context.Database.CanConnect())
            {
                // If the database doesn't exist, create it and apply migrations
                context.Database.EnsureCreated();
            }
        }
    }
} 