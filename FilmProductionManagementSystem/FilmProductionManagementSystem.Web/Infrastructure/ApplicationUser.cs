namespace FilmProductionManagementSystem.Web.Infrastructure;

using Microsoft.AspNetCore.Identity;

using Models;

public class ApplicationUser : IdentityUser
{
    public virtual ICollection<Crew> CrewMembers { get; set; } 
        = new HashSet<Crew>();
    
    public virtual ICollection<Cast> CastMembers { get; set; }
        = new HashSet<Cast>();
    
}