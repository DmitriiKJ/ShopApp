using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopApp.Models;

public class UserContext : IdentityDbContext
{
    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {
        
    }
}