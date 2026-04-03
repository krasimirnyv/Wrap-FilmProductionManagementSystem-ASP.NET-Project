namespace Wrap.Data.Seeding;

using Microsoft.AspNetCore.Identity;

using Interfaces;
using Models.Infrastructure;
using GCommon.UI;

using static GCommon.OutputMessages.ApplicationRoles;

public class ApplicationRoleSeeder(RoleManager<ApplicationRole> roleManager) : IApplicationRoleSeeder
{
    private static IReadOnlyCollection<string> DepartmentAsAbstractRoles
        => CrewRolesDepartmentCatalog.GetRolesByDepartment().Keys.ToArray().AsReadOnly();
    
    public async Task SeedRolesAsync()
    {
        foreach (string role in DepartmentAsAbstractRoles)
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