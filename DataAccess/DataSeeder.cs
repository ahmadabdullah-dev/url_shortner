using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class DataSeeder
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public DataSeeder(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task Seed()
    {
        await SeedRoles();
        await SeedUsers();
    }
    public async Task SeedRoles()
    {
        var roles = new List<AppRole>()
        {
            new() {Name = "Admin", Description = "Admin Role" },
            new() {Name = "Costumer", Description = "Costumer Role" },
        };

        if (!await _roleManager.Roles.AnyAsync())
        {
            foreach (var role in roles)
            {
                await _roleManager.CreateAsync(role);
            }
        }
    }
    public async Task SeedUsers()
    {
        var users = new List<(AppUser user, string role)>()
        {
            (new() {UserName = "admin1", Email= "admin1@test.com", EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddMonths(-6) },"Admin"),
            (new() {UserName = "admin2", Email= "admin2@test.com", EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddMonths(-4) },"Admin"),
            (new() {UserName = "costumer1", Email= "costumer1@test.com", EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddMonths(-2) },"Costumer"),
            (new() {UserName = "costumer2", Email= "costumer2@test.com", EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddMonths(-1) },"Costumer"),
        };

        
        foreach (var (user,role) in users)
        {
            var existingUser = await _userManager.FindByNameAsync(user.UserName!);
         
            if (existingUser == null)
            {
                var result = await _userManager.CreateAsync(user,"Pa$$w0rd");       
               
                if(result.Succeeded)
                    await _userManager.AddToRoleAsync(user, role);
            }

        }

    }
}
