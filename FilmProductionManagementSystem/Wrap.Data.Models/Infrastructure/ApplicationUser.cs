namespace Wrap.Data.Models.Infrastructure;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// Има разделение на снимачен екип и актьори като роли в апликацията
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{

}