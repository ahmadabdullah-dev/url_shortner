using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

namespace DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

                options.User.RequireUniqueEmail = true;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 10;

                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
            services.AddDataProtection();

            return services;
        }
    }
}