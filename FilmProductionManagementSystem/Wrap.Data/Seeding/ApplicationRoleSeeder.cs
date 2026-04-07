namespace Wrap.Data.Seeding;

using Microsoft.AspNetCore.Identity;

using Interfaces;
using Models.Infrastructure;

using static GCommon.ApplicationConstants;
using static GCommon.OutputMessages.ApplicationRoles;

public class ApplicationRoleSeeder(RoleManager<ApplicationRole> roleManager) : IApplicationRoleSeeder
{
    private static readonly string[] Roles =
    [
        IdentityRoles.Filmmaker,
        IdentityRoles.Actor
    ];
    
    public async Task SeedRolesAsync()
    {
        foreach (string role in Roles)
        {
            bool isRoleExists = await roleManager.RoleExistsAsync(role);
            if (!isRoleExists)
            {
                ApplicationRole newRole = new ApplicationRole(role);

                IdentityResult identityRoleResult = await roleManager.CreateAsync(newRole);
                if (!identityRoleResult.Succeeded)
                    throw new Exception(string.Format(RoleSeedingExceptionMessage, role, string.Join(", ", identityRoleResult.Errors.Select(e => e.Description))));
            }
        }
    }
}