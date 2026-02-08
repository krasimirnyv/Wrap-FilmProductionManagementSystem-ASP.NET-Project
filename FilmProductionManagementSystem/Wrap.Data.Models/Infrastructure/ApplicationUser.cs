namespace Wrap.Data.Models.Infrastructure;

using Microsoft.AspNetCore.Identity;

using Models;

/// <summary>
/// Има разделение на снимачен екип и актьори като роли в апликацията
/// </summary>
public class ApplicationUser : IdentityUser
{
    public virtual ICollection<Crew> CrewMembers { get; set; } 
        = new HashSet<Crew>();
    
    public virtual ICollection<Cast> CastMembers { get; set; }
        = new HashSet<Cast>();
    
}