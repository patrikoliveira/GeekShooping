using GeekShopping.IdentityServer.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


public class MySQLContext : IdentityDbContext<ApplicationUser>
{
    public MySQLContext()
    {
    }

    public MySQLContext(DbContextOptions<MySQLContext> options) : base(options)
    {
    }
}

