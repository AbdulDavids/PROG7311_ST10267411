using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROG7311_POE_ST10267411.Data;
using PROG7311_POE_ST10267411.Models;

namespace PROG7311_POE_ST10267411
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline
            ConfigureMiddleware(app, app.Environment);

            // Seed database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    
                    // Apply migrations using our MigrationsManager
                    MigrationsManager.ApplyMigrations(context);
                    
                    // Seed users and roles
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await ApplicationDbContextSeed.SeedDefaultUserAsync(userManager, roleManager);
                    
                    // Seed sample data
                    await ApplicationDbContextSeed.SeedSampleDataAsync(context, userManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "an error occurred while seeding the database");
                }
            }

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext with SQLite
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    configuration.GetConnectionString("DefaultConnection") ?? "Data Source=agrienergyconnect.db"));

            // Add Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    // Password settings for development - adjust for production
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Configure Identity Cookie settings
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            services.AddControllersWithViews();
        }

        private static void ConfigureMiddleware(WebApplication app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}
