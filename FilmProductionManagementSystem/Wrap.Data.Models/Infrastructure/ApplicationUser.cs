namespace Wrap.Data.Models.Infrastructure;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

using Models;

/// <summary>
/// Има разделение на снимачен екип и актьори като роли в апликацията
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public virtual ICollection<Crew> CrewMembers { get; set; } 
        = new HashSet<Crew>();
    
    public virtual ICollection<Cast> CastMembers { get; set; }
        = new HashSet<Cast>();
    
    // [PersonalData]
    // [DataType(DataType.Text)]
    // [MaxLength(200)]
    // public string? FullName { get; set; }
    //
    // [PersonalData]
    // [DataType(DataType.Date)]
    // public DateTime? DateOfBirth { get; set; }
}