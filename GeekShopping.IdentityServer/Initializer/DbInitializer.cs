using System.Security.Claims;
using Duende.IdentityModel;
using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using Microsoft.AspNetCore.Identity;

namespace GeekShopping.IdentityServer.Initializer;

public class DbInitializer(MySQLContext context, UserManager<ApplicationUser> user, RoleManager<IdentityRole> roleManager) : IDbInitializer
{
    public void Initialize()
    {
        if (roleManager.FindByIdAsync(IdentityConfiguration.Admin).Result != null)
            return;

        roleManager.CreateAsync(new IdentityRole(IdentityConfiguration.Admin)).GetAwaiter().GetResult();
        roleManager.CreateAsync(new IdentityRole(IdentityConfiguration.Client)).GetAwaiter().GetResult();

        ApplicationUser admin = new ApplicationUser()
        {
            UserName = "matheus-admin",
            Email = "matheus-admin@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "+55 (14) 99999-9999",
            FirstName = "Matheus",
            LastName = "Admin"
        };

        user.CreateAsync(admin, "Matheus@123").GetAwaiter().GetResult();
        user.AddToRoleAsync(admin, IdentityConfiguration.Admin).GetAwaiter().GetResult();

        var adminClaims = user.AddClaimsAsync(admin, new List<Claim>()
        {
            new Claim(JwtClaimTypes.Name, $"{admin.FirstName} {admin.LastName}"),
            new Claim(JwtClaimTypes.GivenName, $"{admin.FirstName}"),
            new Claim(JwtClaimTypes.FamilyName, $"{admin.LastName}"),
            new Claim(JwtClaimTypes.Role, IdentityConfiguration.Admin)
        }).Result;

        ApplicationUser client = new ApplicationUser()
        {
            UserName = "matheus-client",
            Email = "matheus-client@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "+55 (14) 99999-9999",
            FirstName = "Matheus",
            LastName = "client"
        };

        user.CreateAsync(client, "Matheus@123").GetAwaiter().GetResult();
        user.AddToRoleAsync(client, IdentityConfiguration.Client).GetAwaiter().GetResult();

        var clientnClaims = user.AddClaimsAsync(client, new List<Claim>()
        {
            new Claim(JwtClaimTypes.Name, $"{client.FirstName} {client.LastName}"),
            new Claim(JwtClaimTypes.GivenName, $"{client.FirstName}"),
            new Claim(JwtClaimTypes.FamilyName, $"{client.LastName}"),
            new Claim(JwtClaimTypes.Role, IdentityConfiguration.Client)
        }).Result;
    }
}
