using System.Security.Claims;
using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Model;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace GeekShopping.IdentityServer.Initializer;

public class DbInitializer : IDbInitializer
{
    private readonly MySQLContext context;
    private readonly UserManager<ApplicationUser> user;
    private readonly RoleManager<IdentityRole> role;

    public DbInitializer(MySQLContext context, UserManager<ApplicationUser> user, RoleManager<IdentityRole> role)
    {
        this.context = context;
        this.user = user;
        this.role = role;
    }

    public void Initialize()
    {
        if (role.FindByNameAsync(IdentityConfiguration.Admin).Result != null)
        {
            return;
        }

        role.CreateAsync(new IdentityRole(IdentityConfiguration.Admin)).GetAwaiter().GetResult();
        role.CreateAsync(new IdentityRole(IdentityConfiguration.Client)).GetAwaiter().GetResult();

        ApplicationUser admin = new ApplicationUser()
        {
            UserName = "patrik-admin",
            Email = "patrikro.oliveira@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "+55 (31) 99806-8050",
            FirstName = "Patrik",
            LastName = "Admin"
        };

        user.CreateAsync(admin, "$Patrik123").GetAwaiter().GetResult(); 
        user.AddToRoleAsync(admin, IdentityConfiguration.Admin).GetAwaiter().GetResult();

        var adminClaims = user.AddClaimsAsync(admin, new Claim[]
        {
            new Claim(JwtClaimTypes.Name, $"{admin.FirstName} {admin.LastName}"),
            new Claim(JwtClaimTypes.GivenName, admin.FirstName),
            new Claim(JwtClaimTypes.FamilyName, admin.LastName),
            new Claim(JwtClaimTypes.Role, IdentityConfiguration.Admin)
        }).Result;

        ApplicationUser client = new ApplicationUser()
        {
            UserName = "patrik-client",
            Email = "patrikro.oliveira@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "+55 (31) 99806-8050",
            FirstName = "Patrik",
            LastName = "Client"
        };

        user.CreateAsync(client, "$Patrik123").GetAwaiter().GetResult(); 
        user.AddToRoleAsync(client, IdentityConfiguration.Admin).GetAwaiter().GetResult();

        var clientClaims = user.AddClaimsAsync(client, new Claim[]
        {
            new Claim(JwtClaimTypes.Name, $"{client.FirstName} {client.LastName}"),
            new Claim(JwtClaimTypes.GivenName, client.FirstName),
            new Claim(JwtClaimTypes.FamilyName, client.LastName),
            new Claim(JwtClaimTypes.Role, IdentityConfiguration.Client)
        }).Result;
    }
}

