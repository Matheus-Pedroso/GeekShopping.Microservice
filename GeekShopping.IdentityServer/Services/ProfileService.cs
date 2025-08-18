using System.Security.Claims;
using Duende.IdentityModel;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using GeekShopping.IdentityServer.Model;
using Microsoft.AspNetCore.Identity;

namespace GeekShopping.IdentityServer.Services;

public class ProfileService(
    UserManager<ApplicationUser> userManager,  
    RoleManager<IdentityRole> roleManager, 
    IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory
    ) : IProfileService
{            
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        string id = context.Subject.GetSubjectId();
        ApplicationUser user = await userManager.FindByIdAsync(id);
        ClaimsPrincipal userClaims = await userClaimsPrincipalFactory.CreateAsync(user);

        List<Claim> claims = userClaims.Claims.ToList();
        claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
        claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));

        if (userManager.SupportsUserRole)
        {
            IList<string> roles = await userManager.GetRolesAsync(user);
            foreach (string role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
                if (roleManager.SupportsRoleClaims)
                {
                    IdentityRole identityRole = await roleManager.FindByNameAsync(role);
                    if (identityRole != null)
                    {
                        claims.AddRange(await roleManager.GetClaimsAsync(identityRole));
                    }
                }
            }
        }
        // Filtra as claims de acordo com os tipos solicitados
        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        string id = context.Subject.GetSubjectId();
        ApplicationUser user = await userManager.FindByIdAsync(id);
        context.IsActive = user != null;
    }
}
