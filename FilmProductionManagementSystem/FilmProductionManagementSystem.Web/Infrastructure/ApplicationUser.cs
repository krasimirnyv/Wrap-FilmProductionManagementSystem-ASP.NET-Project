namespace FilmProductionManagementSystem.Web.Infrastructure;

using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public bool IsCrew { get; set; }
    
    public bool IsCast { get; set; }
}