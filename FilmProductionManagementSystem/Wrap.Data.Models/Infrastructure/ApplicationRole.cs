namespace Wrap.Data.Models.Infrastructure;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

using static GCommon.EntityConstants.ApplicationRole;

public class ApplicationRole : IdentityRole<Guid>
{
    [MaxLength(RoleMaxLenght)]
    public string? Role { get; set; }
}