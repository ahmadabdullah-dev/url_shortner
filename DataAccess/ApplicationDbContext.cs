using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<AppUser,AppRole,string>(options)
{
 
  
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

    }
}
